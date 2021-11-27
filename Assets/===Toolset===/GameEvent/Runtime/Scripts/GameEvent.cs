namespace Toolset.GameEvent
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using com.faith.coreconsole;

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
            public Action GameActionReference { get; private set; }

            #endregion

            #region Public Callback

            public GameEventResponse(GameEventListener gameEventListener, Action gameAction)
            {
                GameEventListenerReference = gameEventListener;
                GameActionReference = gameAction;
            }

            #endregion

        }

        #endregion

        #region Private Variables

        private List<GameEventResponse> _listOfGameEventResponse = new List<GameEventResponse>();


        #endregion

        #region Configuretion

        private int IsDuplicationAction(GameEventListener gameEventListener, Action gameAction)
        {
            int numberOfResponse = _listOfGameEventResponse.Count;
            for (int i = 0; i < numberOfResponse; i++)
            {
                if (gameEventListener == _listOfGameEventResponse[i].GameEventListenerReference
                && gameAction == _listOfGameEventResponse[i].GameActionReference)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Public Callback

        public void RegisterEvent(GameEventListener gameEventListener, Action gameAction)
        {
            if (IsDuplicationAction(gameEventListener, gameAction) == -1)
            {
                GameEventResponse newGameEventResponse = new GameEventResponse(gameEventListener, gameAction);
                _listOfGameEventResponse.Add(newGameEventResponse);
            }

        }

        public void UnregisterEvent(GameEventListener gameEventListener, Action gameAction)
        {
            int index = IsDuplicationAction(gameEventListener, gameAction);
            if (index != -1)
            {
                _listOfGameEventResponse.RemoveAt(index);
                _listOfGameEventResponse.TrimExcess();
            }
        }

        public void Raise() {

            CoreConsole.Log(string.Format("EventRaised : {0}", name), Color.magenta, "GameEvent");

            int numberOfEvent = _listOfGameEventResponse.Count;
            for (int i = numberOfEvent - 1; i >= 0; i--)
                _listOfGameEventResponse[i].GameActionReference?.Invoke();
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

