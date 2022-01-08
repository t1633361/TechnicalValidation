using System;
using System.Collections;
using UnityEngine;

namespace Coroutine
{
    public class CoroutineTest : MonoBehaviour
    {
        private bool _copyCrash;
        private bool _copyYieldBreak;
    
        private void Copy()
        {
            StartCoroutine(AsyncCopy());
        }

        private IEnumerator AsyncCopy()
        {
            yield return null;
            for (var i = 0; i < 5; ++i)
            {
                Debug.Log($"Copy:{i}");
                yield return null;
                if (_copyYieldBreak)
                    yield break;
            }
        }

        private void Paste()
        {
            StartCoroutine(AsyncPaste());
        }

        private IEnumerator AsyncPaste()
        {
            yield return null;
            for (var i = 0; i < 5; ++i)
            {
                Debug.Log($"Paste:{i}");
                yield return null;
            }
            if (_copyCrash)
                throw new NotImplementedException();
        }

        private void Duplicate()
        {
            StartCoroutine(AsyncDuplicate());
        }

        private IEnumerator AsyncDuplicate()
        {
            yield return AsyncCopy();
            yield return AsyncPaste();
        }

        public void OnGUI()
        {

            if (GUILayout.Button("Copy"))
            {
                Copy();
            }
            if (GUILayout.Button("Paste"))
            {
                Paste();
            }
            if (GUILayout.Button("Duplicate"))
            {
                Duplicate();
            }
            if (GUILayout.Button("DuplicateCrash"))
            {
                _copyCrash = true;
                
                Duplicate();
            }
            if (GUILayout.Button("DuplicateYieldBreak"))
            {
                _copyYieldBreak = true;
                Duplicate();
            }
        }
    }
}
