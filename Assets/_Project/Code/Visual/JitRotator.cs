using UnityEngine;

namespace Code.Visual
{
    public class JitRotator : MonoBehaviour
    {
        public float Angle = 10f;
        public float Period = 0.15f;

        private float timer;
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= Period)
            {
                timer = 0;
                transform.Rotate(0, 0, Angle);
            }
        }
    }
}