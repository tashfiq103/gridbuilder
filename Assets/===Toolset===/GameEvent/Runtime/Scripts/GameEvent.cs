namespace Toolset.GameEvent
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "GameEvent", menuName = "FAITH/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        #region Custom Variables

        private class GameEventResponse
        {

            #region Public Variables

            public GameEventListener GameEventListenerReference { get; private set; }
            public Action GameEventResponseReference { get; private set; }

            #endregion

            #region Public Callback

            public bool IsSameGameEventListener(GameEventListener gameEventListener)
            {
                if (GameEventListenerReference == gameEventListener)
                    return true;

                return false;
            }

            public GameEventResponse(GameEventListener gameEventListener, Action gameEventResponse)
            {
                GameEventListenerReference = gameEventListener;
                GameEventResponseReference = gameEventResponse;
            }

            #endregion

        }

        #endregion

        #region Private Variables

        private List<GameEventResponse> _listOfGameEventResponse                                = new List<GameEventResponse>();
        private Dictionary<GameEventListener, GameEventResponse> _trackerForGameEventListener   = new Dictionary<GameEventListener, GameEventResponse>();
        

        #endregion


        #region Public Callback

        public void RegisterEvent(GameEventListener gameEventListener, Action gameEventResponse)
        {
            if (!_trackerForGameEventListener.ContainsKey(gameEventListener))
            {
                GameEventResponse newGameEventResponse = new GameEventResponse(gameEventListener, gameEventResponse);

                _listOfGameEventResponse.Add(newGameEventResponse);
                _trackerForGameEventListener.Add(gameEventListener, newGameEventResponse);
            }

        }

        public void UnregisterEvent(GameEventListener gameEventListener)
        {
            if (_trackerForGameEventListener.ContainsKey(gameEventListener))
            {
                GameEventResponse removingGameEventResponse;
                if (_trackerForGameEventListener.TryGetValue(gameEventListener, out removingGameEventResponse)) {

                    _listOfGameEventResponse.Remove(removingGameEventResponse);
                    _trackerForGameEventListener.Remove(gameEventListener);
                }
            }
        }

        public void Raise() {

            int numberOfEvent = _listOfGameEventResponse.Count;
            for (int i = numberOfEvent - 1; i >= 0; i--)
                _listOfGameEventResponse[i].GameEventResponseReference?.Invoke();
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor
    {
        #region Private Variables

        private GameEvent _reference;

        #endregion

        #region Editor

        private void OnEnable()
        {
            _reference = (GameEvent)target;

            if (_reference == null)
                return;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            if (GUILayout.Button("Raise"))
            {
                _reference.Raise();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
#endif
}

