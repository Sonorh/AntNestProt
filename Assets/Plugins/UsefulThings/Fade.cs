using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PM.UsefulThings
{
    public class Fade : MonoBehaviour
    {
		public event System.Action OnFinish;

        private Image image
        {
            get
            {
                if (__image == null)
                    __image = this.GetComponent<Image>();

                return __image;

            }
        }
        private bool isFadeIn { get; set; }
        private bool isStart { get; set; }
        private float speed { get; set; }

        private float blackStartTime = 0;
        private Image __image { get; set; }

        // Use this for initialization
        void Awake()
        {
            speed = 1.5f;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isStart)
                return;

            blackStartTime += Time.deltaTime * speed;

            if (blackStartTime > 1)
            {
                this.SetSolid(isFadeIn);
                isStart = false;
				OnFinish?.Invoke();
			}
            else
            {
                float lerp;
                if (isFadeIn)
                    lerp = Mathf.Lerp(0, 1, blackStartTime > 1 ? 1 : blackStartTime);
                else
                    lerp = Mathf.Lerp(1, 0, blackStartTime > 1 ? 1 : blackStartTime);
                image.color = new Color(image.color.r, image.color.g, image.color.b, lerp);
            }
        }

        public void SetBlack()
        {
            this.transform.parent.gameObject.SetActive(true);
            isFadeIn = true;
            isStart = true;
        }
        public void SetWhite()
        {
            this.transform.parent.gameObject.SetActive(true);
            isFadeIn = false;
            isStart = true;
        }


        public void SetSolid(bool isBlack)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, isBlack ? 1 : 0);
            if (isBlack)
                this.transform.parent.gameObject.SetActive(true);
            else
                this.transform.parent.gameObject.SetActive(false);
        }


        public void SetSpeed(float sec)
        {
            speed = sec;
        }
    }
}