using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// # Summary  
/// A component cache that stores references to frequently accessed <see cref="Component"/>s
/// and exposes a type-safe retrieval API.
/// 자주 사용되는 <see cref="Component"/> 참조를 캐싱하여
/// 안전한 타입조회 API를 제공한다.
///
/// ## Usage
/// - Only one provider per GameObject (<c>[DisallowMultipleComponent]</c>).
/// - Register components in the Inspector list or via <see cref="Add"/> at any time.
/// - Call <see cref="GetAs{T}"/> with a concrete or interface type for an O(1) look-up.
/// ## 사용 방법
/// - <c>[DisallowMultipleComponent]</c>로 게임 오브젝트당 하나만 붙을 수 있다.
/// - 인스펙터 리스트 또는 <see cref="Add"/>로 컴포넌트를 등록한다. <see cref="Add"/>는 시점에 관계없이 호출할 수 있다.
/// - 게임 로직에서 구체 타입 또는 인터페이스 타입으로 <see cref="GetAs{T}"/>를 호출해 O(1)로 컴포넌트를 가져온다.
///
/// ## Features
/// - Converts an editable <c>List&lt;Component&gt;</c> into an O(1) <c>Dictionary</c> at <c>Awake</c>.
/// - Maps each component under its concrete type and all implemented interfaces.
/// - Integrates with <see cref="ComponentRegistry"/> for global look-ups.
/// - Safe add-after-initialization handling.
/// ## 기능
/// - 인스펙터에서 수정 가능한 <c>List&lt;Component&gt;</c>를 <c>Awake</c>에서 O(1) 딕셔너리로 변환한다.
/// - 각 컴포넌트를 구체 타입과 구현한 모든 인터페이스로 매핑한다.
/// - 전역 조회를 위해 <see cref="ComponentRegistry"/>와 연동한다.
/// - 초기화 이후의 추가 요청도 안전하게 처리한다.
/// </summary>
[DisallowMultipleComponent]
public class ComponentProvider : MonoBehaviour
{
    [Tooltip("Components registered at Awake. 인스펙터에서 등록할 컴포넌트 목록.")]
    [SerializeField] private List<Component> _components = new();

    private readonly Dictionary<Type, Component> _componentMap = new();
    private bool _isInitialized = false;

    private void Awake() => Initialize();
    private void OnDestroy() => CleanUp();

    /// <summary>
    /// Adds a component reference either to the staging list or directly to the dictionary  
    /// depending on the initialization state.  
    /// 초기화 상태에 따라 컴포넌트를 대기 리스트 혹은 즉시 딕셔너리에 추가한다.
    /// </summary>
    /// <param name="component">Component to register. 등록할 컴포넌트.</param>
    public void Add(Component component)
    {
        if (component == null) return;

        if (_isInitialized) AddToDictionary(component);
        else _components.Add(component);
    }

    /// <summary>
    /// Retrieves the cached component of type <typeparamref name="T"/>.  
    /// 타입 <typeparamref name="T"/>의 캐시된 컴포넌트를 반환한다.
    /// </summary>
    /// <typeparam name="T">
    /// Desired concrete component or interface type (e.g. <c>IDamageable</c>).
    /// 조회할 구체 컴포넌트 또는 인터페이스 타입.
    /// </typeparam>
    /// <returns>
    /// Cached component if present; otherwise <c>null</c>.  
    /// 캐시된 컴포넌트가 존재하면 반환하고, 없으면 <c>null</c>을 반환한다.
    /// </returns>
    public T GetAs<T>() where T : class
    {
        return _componentMap.TryGetValue(typeof(T), out Component component) ? component as T : null;
    }

    
    /// <summary>
    /// Initializes the internal dictionary and registers this provider globally.  
    /// 내부 딕셔너리를 초기화하고 전역 레지스트리에 등록한다.
    /// </summary>
    private void Initialize()
    {
        foreach (Component component in _components)
        {
            AddToDictionary(component);
        }

        ComponentRegistry.Register(this);
        _isInitialized = true;
    }

    /// <summary>
    /// Unregisters this provider and releases its dictionary.  
    /// 레지스트리에서 자신을 제거하고 딕셔너리를 비운다.
    /// </summary>
    private void CleanUp()
    {
        ComponentRegistry.Unregister(this);
        _componentMap.Clear();
    }

    /// <summary>
    /// Maps a component under its concrete type and every interface it implements.
    /// The first component registered for a type wins.
    /// 컴포넌트를 구체 타입과 구현한 모든 인터페이스로 매핑한다.
    /// 같은 타입은 먼저 등록된 컴포넌트가 우선한다.
    /// </summary>
    /// <param name="component">Component to insert. 추가할 컴포넌트.</param>
    private void AddToDictionary(Component component)
    {
        if (component == null) return;

        Type type = component.GetType();
        _componentMap.TryAdd(type, component);

        foreach (Type interfaceType in type.GetInterfaces())
        {
            _componentMap.TryAdd(interfaceType, component);
        }
    }
}