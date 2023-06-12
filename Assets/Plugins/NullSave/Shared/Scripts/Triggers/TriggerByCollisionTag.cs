using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    public class TriggerByCollisionTag : MonoBehaviour
    {

        #region Variables

        public string compareTag;
        public bool allowTriggers;

        public UnityEvent onEnter, onStay, onExit;

        #endregion

        #region Unity Events

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag(compareTag))
            {
                onEnter?.Invoke();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onEnter?.Invoke();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onExit?.Invoke();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onExit?.Invoke();
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onStay?.Invoke();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onStay?.Invoke();
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onEnter?.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onExit?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onExit?.Invoke();
            }
        }

        private void OnTriggerStay(Collider collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onStay?.Invoke();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(compareTag))
            {
                onStay?.Invoke();
            }
        }

        #endregion

    }
}