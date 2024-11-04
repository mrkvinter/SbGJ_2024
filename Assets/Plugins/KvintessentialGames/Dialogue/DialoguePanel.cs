using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using KvinterGames;
using KvintessentialGames.TextAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.UI
{
  public class DialoguePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private PlayableTextAnimation fallLettersAnimation;

        private List<Color32[]> colors;
        private bool isShowing;
        private bool isSkipping;
        private bool updateColors;
        private HashSet<char> additionalDelayChars = new() {'.', ',', '!', '?'};

        public void ShowDialogue(string text)
        {
            ShowSequence(Dialogue
                .Create()
                .Text(text)).Forget();
        }

        public void ShowDialogue(Dialogue sequence, Action onComplete = null)
        {
            if (isShowing)
            {
                UniTask.Void(async () =>
                {
                    await UniTask.WaitWhile(() => isShowing);
                    await SafeShowSequence(sequence, onComplete);

                });
            }
            else
            { 
                SafeShowSequence(sequence, onComplete).Forget();
            }
        }
        
        private async UniTask SafeShowSequence(Dialogue sequence, Action onComplete = null)
        {
            try
            {
                await ShowSequence(sequence, onComplete);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                isShowing = false;
            }
        }

        public async UniTask ShowDialogueAsync(Dialogue sequence)
        {
            await UniTask.WaitWhile(() => isShowing);
            await ShowSequence(sequence);
        }
        
        public void Skip()
        {
            if (isShowing)
            {
                isSkipping = true;
            }
        }

        private async UniTask ShowSequence(Dialogue sequence, Action onComplete = null)
        {
            isShowing = true;

            var sb = new StringBuilder();
            var nextTimeSound = 0f;
            var characterIndex = 0;
            var previousText = new StringBuilder();

            for (var i = 0; i < sequence.Stages.Count; i++)
            {
                var stage = sequence.Stages[i];
                if (stage.Type == DialogueStageType.Text)
                {
                    var delay = stage.Speed switch
                    {
                        DialogueSpeed.Fast => 25,
                        DialogueSpeed.Medium => 45,
                        DialogueSpeed.Slow => 80,
                        DialogueSpeed.Instant => 0,
                        _ => 45
                    };

                    var previousUnderlineText = text.textInfo.characterInfo[characterIndex].underlineVertexIndex;
                    for (var j = 0; j < stage.Text.Length; j++, characterIndex++)
                    {
                        var characterInfo = text.textInfo.characterInfo[characterIndex];
                        if (!characterInfo.isVisible){
                            continue;
                        }

                        while (char.ToLower(stage.Text[j]) != char.ToLower(characterInfo.character))
                        {
                            j++;
                            
                            if (j >= stage.Text.Length)
                            {
                                break;
                            }
                        }
                        
                        if (j >= stage.Text.Length)
                        {
                            break;
                        }

                        var color = characterInfo.color;
                        color.a = 255;
                        var meshIndex = characterInfo.materialReferenceIndex;
                        var vertexIndex = characterInfo.vertexIndex;

                        var meshColors = colors[meshIndex];
                        meshColors[vertexIndex + 0] = color;
                        meshColors[vertexIndex + 1] = color;
                        meshColors[vertexIndex + 2] = color;
                        meshColors[vertexIndex + 3] = color;
                        
                        updateColors = true;
                        
                        if (characterInfo.underlineVertexIndex != 0)
                        {
                            if (previousUnderlineText == 0)
                            {
                                previousUnderlineText = characterInfo.underlineVertexIndex;
                            }
                            
                            if (previousUnderlineText != characterInfo.underlineVertexIndex)
                            {
                                var underlineVertexIndex = previousUnderlineText;
                                meshColors[underlineVertexIndex + 0] = color;
                                meshColors[underlineVertexIndex + 1] = color;
                                meshColors[underlineVertexIndex + 2] = color;
                                meshColors[underlineVertexIndex + 3] = color;
                                meshColors[underlineVertexIndex + 4] = color;
                                meshColors[underlineVertexIndex + 5] = color;
                                meshColors[underlineVertexIndex + 6] = color;
                                meshColors[underlineVertexIndex + 7] = color;
                                
                                previousUnderlineText = characterInfo.underlineVertexIndex;
                            }
                        }
                        else if (previousUnderlineText != 0)
                        {
                            var underlineVertexIndex = previousUnderlineText;
                            meshColors[underlineVertexIndex + 0] = color;
                            meshColors[underlineVertexIndex + 1] = color;
                            meshColors[underlineVertexIndex + 2] = color;
                            meshColors[underlineVertexIndex + 3] = color;
                            meshColors[underlineVertexIndex + 4] = color;
                            meshColors[underlineVertexIndex + 5] = color;
                            meshColors[underlineVertexIndex + 6] = color;
                            meshColors[underlineVertexIndex + 7] = color;
                            
                            previousUnderlineText = 0;
                        }

                        var c = stage.Text[j];
                        if (stage.WithSpeaker && nextTimeSound <= Time.time && char.IsLetter(c))
                        {
                            var speaker = sequence.OverrideSpeaker ?? "Default";
                            var sound = SoundController.Instance.PlaySound(
                                $"Typing_{speaker}", 0.1f, 0.5f, pitch: isSkipping ? 1.5f : 1f);
                                
                            var length = sound != null ? sound.length : 0.1f;
                            nextTimeSound = Time.time + length * (isSkipping ? 0.5f : 1f);
                        }

                        if (delay != 0)
                        {
                            var multiplier = additionalDelayChars.Contains(c) ? 4f : 1f;
                            multiplier *= isSkipping ? 0.1f : 1;
                            await UniTask.Delay((int)(delay * multiplier));
                        }
                    }
                }

                if (stage.Type == DialogueStageType.Delay)
                {
                    isSkipping = false;
                    var timer = 0f;
                    while (timer < stage.Delay && !isSkipping)
                    {
                        timer += Time.deltaTime * 1000;
                        await UniTask.Yield();
                    }
                }

                if (stage.Type == DialogueStageType.Clear)
                {
                    sb.Clear();
                    isSkipping = false;
                    previousText.Clear();
                    characterIndex = 0;
                    for (var j = i + 1; j < sequence.Stages.Count; j++)
                    {
                        var sequenceStage = sequence.Stages[j];
                        if (sequenceStage.Type == DialogueStageType.Clear)
                        {
                            break;
                        }

                        if (sequenceStage.Type == DialogueStageType.Text)
                        {
                            previousText.Append(sequenceStage.Text);
                        }
                    }
                    ClearTextColors();
                    text.text = previousText.ToString();
                    text.ForceMeshUpdate();
                    if (previousText.Length != 0)
                        await UniTask.WaitWhile(() => text.textInfo.characterCount == 0);
                    
                    colors = new List<Color32[]>();
                    for (var j = 0; j < text.textInfo.meshInfo.Length; j++)
                    {
                        var array = new Color32[text.textInfo.meshInfo[j].colors32.Length];
                        Array.Copy(text.textInfo.meshInfo[j].colors32, array, text.textInfo.meshInfo[j].colors32.Length);
                        for (var i1 = 0; i1 < array.Length; i1++)
                        {
                            array[i1] = new Color32(array[i1].r, array[i1].g, array[i1].b, 0);
                        }
                        colors.Add(array);
                    }
                }

                if (stage.Type == DialogueStageType.Animation)
                {
                    fallLettersAnimation.Play();
                    await UniTask.WaitWhile(() => fallLettersAnimation.IsPlaying);
                }
            }

            onComplete?.Invoke();
            isShowing = false;
        }
        
        private void ClearTextColors()
        {
            if (text.textInfo == null)
            {
                return;
            }

            for (var i = 0; i < text.textInfo.meshInfo.Length; i++)
            {
                var colors32 = text.textInfo.meshInfo[i].colors32;
                for (var j = 0; j < colors32.Length; j++)
                {
                    colors32[j].a = 0;
                }
                text.UpdateGeometry(text.textInfo.meshInfo[i].mesh, i);
            }
        }

        private void LateUpdate()
        {
            if (colors == null || text.textInfo.meshInfo.Length != colors.Count)
            {
                return;
            }

            if (updateColors)
            {
                UpdateTextColors();
                updateColors = false;
            }
            else
            {
                UpdateTransparentColor();
            }
        }
        
        
        private void UpdateTransparentColor()
        {
            for (var i = 0; i < text.textInfo.meshInfo.Length; i++)
            {
                var colors32 = text.textInfo.meshInfo[i].colors32;
                var meshColors = colors[i];

                for (var j = 0; j < meshColors.Length; j++)
                {
                    if (meshColors[j].a != 0)
                    {
                        continue;
                    }
                    colors32[j] = meshColors[j];
                }
            }
            
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        private void UpdateTextColors()
        {
            for (var i = 0; i < text.textInfo.meshInfo.Length; i++)
            {
                var colors32 = text.textInfo.meshInfo[i].colors32;
                var meshColors = colors[i];

                for (var j = 0; j < meshColors.Length; j++)
                {
                    colors32[j] = meshColors[j];
                }
            }
            
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        public void Clear()
        {
            ShowDialogue(Dialogue.Create());
        }
    }
}