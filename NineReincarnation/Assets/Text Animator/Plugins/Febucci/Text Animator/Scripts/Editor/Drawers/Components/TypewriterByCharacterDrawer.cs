using UnityEditor;

namespace Febucci.UI.Core.Editors
{
    [CustomEditor(typeof(TypewriterByCharacter), true)]
    class TypewriterByCharacterDrawer : TypewriterCoreDrawer
    {
        SerializedProperty waitForNormalChars;
        SerializedProperty waitLong;
        SerializedProperty waitMiddle;
        SerializedProperty avoidMultiplePunctuationWait;
        SerializedProperty waitForNewLines;
        SerializedProperty waitForLastCharacter;

        PropertyWithDifferentLabel useTypewriterWaitForDisappearances;
        PropertyWithDifferentLabel disappearanceWaitTime;
        PropertyWithDifferentLabel disappearanceSpeedMultiplier;

        protected override void OnEnable()
        {
            base.OnEnable();

            waitForNormalChars = serializedObject.FindProperty("waitForNormalChars");
            waitLong = serializedObject.FindProperty("waitLong");
            waitMiddle = serializedObject.FindProperty("waitMiddle");
            avoidMultiplePunctuationWait = serializedObject.FindProperty("avoidMultiplePunctuationWait");
            waitForNewLines = serializedObject.FindProperty("waitForNewLines");
            waitForLastCharacter = serializedObject.FindProperty("waitForLastCharacter");
            useTypewriterWaitForDisappearances = new PropertyWithDifferentLabel(serializedObject, "useTypewriterWaitForDisappearances", "Use Typewriter Wait Times");
            disappearanceSpeedMultiplier = new PropertyWithDifferentLabel(serializedObject, "disappearanceSpeedMultiplier", "Typewriter Speed Multiplier");
            disappearanceWaitTime = new PropertyWithDifferentLabel(serializedObject, "disappearanceWaitTime", "Disappearances Wait");
        }

        protected override string[] GetPropertiesToExclude()
        {
            string[] newProperties = new string[] {
                "script",
                "waitForNormalChars",
                "waitLong",
                "waitMiddle",
                "avoidMultiplePunctuactionWait",
                "avoidMultiplePunctuationWait",
                "waitForNewLines",
                "waitForLastCharacter",
                "useTypewriterWaitForDisappearances",
                "disappearanceSpeedMultiplier",
                "disappearanceWaitTime",
            };

            string[] baseProperties = base.GetPropertiesToExclude();

            string[] mergedArray = new string[newProperties.Length + baseProperties.Length];

            for (int i = 0; i < baseProperties.Length; i++)
            {
                mergedArray[i] = baseProperties[i];
            }

            for (int i = 0; i < newProperties.Length; i++)
            {
                mergedArray[i + baseProperties.Length] = newProperties[i];
            }

            return mergedArray;
        }

        protected override void OnTypewriterSectionGUI()
        {
            EditorGUILayout.PropertyField(waitForNormalChars);
            EditorGUILayout.PropertyField(waitLong);
            EditorGUILayout.PropertyField(waitMiddle);

            EditorGUILayout.PropertyField(avoidMultiplePunctuationWait);
            EditorGUILayout.PropertyField(waitForNewLines);
            EditorGUILayout.PropertyField(waitForLastCharacter);
        }

        protected override void OnDisappearanceSectionGUI()
        {
            useTypewriterWaitForDisappearances.PropertyField();

            if (useTypewriterWaitForDisappearances.property.boolValue)
                disappearanceSpeedMultiplier.PropertyField();
            else
                disappearanceWaitTime.PropertyField();

        }
    }
}