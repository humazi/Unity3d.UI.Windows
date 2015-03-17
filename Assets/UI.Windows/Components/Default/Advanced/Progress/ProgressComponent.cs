﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows.Extensions;
using UnityEngine.UI.Windows.Components.Events;
using UnityEngine.Events;

namespace UnityEngine.UI.Windows.Components {

	public class ProgressComponent : WindowComponent {
		
		public float duration = 0f;
		public float minNormalizedValue = 0f;
		public Extensions.Slider bar;

		public bool continious;
		[Range(0f, 1f)]
		public float continiousWidth = 0.4f;

		public float continiousAngleStep = 0f;

		private Color color;
		public Image[] images;
		
		private ComponentEvent<float> callback = new ComponentEvent<float>();
		private ComponentEvent<ProgressComponent, float> callbackButton = new ComponentEvent<ProgressComponent, float>();

		private float currentValue = 0f;

		public override void OnInit() {

			base.OnInit();

			this.currentValue = (this.bar != null) ? this.bar.value : 0f;
			this.bar.continuousAngleStep = this.continiousAngleStep;

			if (this.continious == false) {

				this.SetAsDefault();

			} else {

				this.SetAsContinuous(this.continiousWidth);

			}
			
			this.bar.onValueChanged.RemoveListener(this.OnValueChanged_INTERNAL);
			this.bar.onValueChanged.AddListener(this.OnValueChanged_INTERNAL);

		}
		
		public virtual void SetCallback(UnityAction<float> callback) {

			this.callback.AddListenerDistinct(callback);
			this.callbackButton.RemoveAllListeners();

		}
		
		public virtual void SetCallback(UnityAction<ProgressComponent, float> callback) {
			
			this.callbackButton.AddListenerDistinct(callback);
			this.callback.RemoveAllListeners();

		}

		private void OnValueChanged_INTERNAL(float value) {
			
			this.currentValue = value;

			if (this.callback != null) this.callback.Invoke(this.currentValue);
			if (this.callbackButton != null) this.callbackButton.Invoke(this, this.currentValue);

		}

		public virtual void SetColor(Color color) {
			
			this.color = color;
			for (int i = 0; i < this.images.Length; ++i) this.images[i].color = color;
			
		}
		
		public Color GetColor() {
			
			return this.color;

		}

		public void SetAsContinuous(float width = 0.4f) {
			
			this.bar.continuous = true;
			this.bar.continuousWidth = width;

			this.SetAnimation();

		}

		public void SetAsDefault() {

			this.bar.continuous = false;

			this.BreakAnimation();

		}

		public void BreakAnimation() {

			TweenerGlobal.instance.removeTweens(this);

		}

		public void SetAnimation() {

			if (this.continious == true && this.bar.canReceiveEvents == false) {

				TweenerGlobal.instance.removeTweens(this);

				if (this.bar.IsFilled() == true) {
					
					TweenerGlobal.instance.addTween(this, this.duration, 0f, 1f).tag(this).ease(ME.Ease.Linear).repeat().onUpdate((obj, value) => {
						
						if (obj != null) {
							
							obj.SetValue(value, immediately: true);
							
						}
						
					});

				} else {

					TweenerGlobal.instance.addTween(this, this.duration, 0f, 1f).tag(this).ease(ME.Ease.InOutElastic).reflect().repeat().onUpdate((obj, value) => {

						if (obj != null) {

							obj.SetValue(value, immediately: true);

						}

					});

				}

			}

			if (this.bar != null && this.bar.continuous == true) {

				var delta = Time.unscaledDeltaTime;

				var value = this.bar.normalizedValue;
				value += delta;


			}

		}

		public void SetValue(float value, bool immediately = false) {

			if (this.continious == true && immediately == false) return;

			value = Mathf.Clamp01(value);

			if (this.bar != null) {

				if (immediately == false && this.duration > 0f) {

					TweenerGlobal.instance.removeTweens(this.bar);
					TweenerGlobal.instance.addTween(this.bar, this.duration, this.currentValue, value).onUpdate((obj, val) => {

						if (obj != null) {

							this.SetValue_INTERNAL(val);

						}

					}).tag(this.bar);

				} else {

					this.SetValue_INTERNAL(value);

				}

			}

		}

		private void SetValue_INTERNAL(float value) {
			
			this.currentValue = value;
			this.bar.value = value;

			if (this.continious == false) {

				this.bar.normalizedValue = Mathf.Clamp(this.bar.normalizedValue, this.minNormalizedValue, 1f);

			}

		}

		public float GetValue() {

			return (this.bar != null) ? this.bar.value : 0f;

		}

		#if UNITY_EDITOR
		public override void OnValidate() {

			base.OnValidate();

			ME.Utilities.FindReference<Extensions.Slider>(this, ref this.bar);

			if (this.bar != null) {
				
				this.bar.minValue = 0f;
				this.bar.maxValue = 1f;
				this.bar.wholeNumbers = false;
				
			}

		}
		#endif

	}

}