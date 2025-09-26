using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
	private class Subscription
	{
		public Delegate Callback;
		public int Priority;
	}

	private static readonly Dictionary<Type, List<Subscription>> Events = new();
	private static readonly Dictionary<Type, object> LastEvents = new();

	public static bool DebugLogging = false;

	public static void Subscribe<T>(Action<T> listener, int priority = 0, bool replayLast = false)
	{
		Type type = typeof(T);

		if (!Events.TryGetValue(type, out List<Subscription> subs))
		{
			subs = new();
			Events[type] = subs;
		}

		subs.Add(new() { Callback = listener, Priority = priority, });

		// Keep subscribers sorted by priority (high → low)
		subs.Sort((a, b) => b.Priority.CompareTo(a.Priority));

		// If sticky event exists, immediately replay
		if (replayLast && LastEvents.TryGetValue(type, out object lastEvent)) listener((T)lastEvent);
	}

	public static void Unsubscribe<T>(Action<T> listener)
	{
		Type type = typeof(T);
		if (!Events.TryGetValue(type, out List<Subscription> subs)) return;
		subs.RemoveAll(s => s.Callback.Equals(listener));

		if (subs.Count != 0) return;
		Events.Remove(type);
	}

	public static void Publish<T>(T publishedEvent, bool remember = false)
	{
		if (DebugLogging)
			Debug.Log($"[EventBus] {typeof(T).Name} fired: {publishedEvent}");

		if (remember)
			LastEvents[typeof(T)] = publishedEvent;

		if (!Events.TryGetValue(typeof(T), out List<Subscription> subs)) return;

		// To prevent "Collection was modified" exceptions, since C#’s foreach does not allow modification while iterating.
		Subscription[] snapshot = subs.ToArray();
		foreach (Subscription sub in snapshot) (sub.Callback as Action<T>)?.Invoke(publishedEvent);
	}

	public static void Clear()
	{
		Events.Clear();
	}

	public static void ClearSticky()
	{
		LastEvents.Clear();
	}
}
