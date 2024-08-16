using System.Collections;
using TMPro;
using UnityEngine;

public class TextWaveAnimation : MonoBehaviour
{
    [Header("Text Settings")]
    public TMP_Text textComponent;

    [Header("Animation Settings")]
    public float waveAmplitude;
    public float waveSpeed;
    public float letterDelay;

    private TMP_TextInfo textInfo;

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }

        textInfo = textComponent.textInfo;
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        while (true)
        {
            textComponent.ForceMeshUpdate();
            int characterCount = textInfo.characterCount;

            if (characterCount == 0) yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3 offset = new Vector3(0, Mathf.Sin(Time.time * waveSpeed + i * letterDelay) * waveAmplitude, 0);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            yield return null;
        }
    }
}
