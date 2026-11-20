using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// # Summary  
/// A static registry that maps each <see cref="GameObject"/> to one <see cref="ComponentProvider"/>  
/// so that expensive component look-ups are replaced with cached access.  
/// 게임 오브젝트마다 하나의 <see cref="ComponentProvider"/>를 매핑해  
/// 비용이 큰 GetComponent 호출을 캐싱한다.  
///  
/// ## Usage
/// - <see cref="ComponentProvider"/> calls <see cref="Register"/> in <c>Awake</c>
///   and <see cref="Unregister"/> in <c>OnDestroy</c> automatically.
/// - Use <see cref="GetAs{T}"/> instead of <c>GetComponent&lt;T&gt;</c>.
/// - Disabled objects stay registered until destroyed.
/// ## 사용 방법
/// - <see cref="ComponentProvider"/>가 <c>Awake</c>에서 <see cref="Register"/>,
///   <c>OnDestroy</c>에서 <see cref="Unregister"/>를 자동으로 호출한다.
/// - <c>GetComponent&lt;T&gt;</c> 대신 <see cref="GetAs{T}"/>를 사용해 캐시된 컴포넌트를 조회한다.
/// - 비활성화된 오브젝트도 파괴되기 전까지는 등록 상태로 남는다.
///
/// ## Features
/// - O(1) look-up time for cached components.
/// - Simple and minimal public API.
/// - Not thread-safe; use from the Unity main thread only.
/// ## 기능
/// - 캐시된 컴포넌트를 O(1)로 조회한다.
/// - 간결한 공개 API로 직관적인 사용성을 제공한다.
/// - 스레드 안전하지 않으므로 Unity 메인 스레드에서만 사용한다.
/// </summary>
public static class ComponentRegistry
{
    private static readonly Dictionary<GameObject, ComponentProvider> _providers = new();

    /// <summary>
    /// Registers a <see cref="ComponentProvider"/> if its host <see cref="GameObject"/> is not yet tracked.  
    /// 프로바이더의 게임 오브젝트가 아직 등록되지 않았으면 레지스트리에 추가한다.
    /// </summary>
    /// <param name="provider">The provider to register. 등록할 프로바이더.</param>
    public static void Register(ComponentProvider provider) => _providers.TryAdd(provider.gameObject, provider);

    /// <summary>
    /// Removes the specified provider from the registry, only if it is the one registered for its GameObject.
    /// 지정된 프로바이더가 해당 게임 오브젝트에 등록된 프로바이더일 때만 레지스트리에서 제거한다.
    /// </summary>
    /// <param name="provider">The provider to remove. 제거할 프로바이더.</param>
    public static void Unregister(ComponentProvider provider)
    {
        GameObject go = provider.gameObject;
        if (_providers.TryGetValue(go, out ComponentProvider registered) && registered == provider)
        {
            _providers.Remove(go);
        }
    }

    /// <summary>
    /// Clears the entire registry. Use with caution during scene transitions.  
    /// 레지스트리를 초기화한다. 씬 전환 시 주의해서 사용한다.
    /// </summary>
    public static void Clear() => _providers.Clear();

    /// <summary>
    /// Retrieves a cached component of type <typeparamref name="T"/> from the given <see cref="GameObject"/>.  
    /// 지정된 <see cref="GameObject"/>에서 타입 <typeparamref name="T"/>의 캐시된 컴포넌트를 반환한다.
    /// </summary>
    /// <typeparam name="T">Concrete component or interface type. 가져올 구체 컴포넌트 또는 인터페이스 타입.</typeparam>
    /// <param name="gameObject">Host GameObject key. 조회할 게임 오브젝트.</param>
    /// <returns>
    /// The cached component if found; otherwise <c>null</c>.  
    /// 캐시된 컴포넌트가 존재하면 반환하고, 없으면 <c>null</c>을 반환한다.
    /// </returns>
    public static T GetAs<T>(GameObject gameObject) where T : class
    {
        if (gameObject == null) return null;

        return _providers.TryGetValue(gameObject, out ComponentProvider provider) ? provider.GetAs<T>() : null;
    }
}