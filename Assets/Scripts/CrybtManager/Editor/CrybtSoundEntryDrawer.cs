using UnityEditor;
using UnityEngine;

// =============================================================
// CRYBT SOUND ENTRY DRAWER
// =============================================================
//
// Makes a row of the Crybt sound table read "Card Hover"
// instead of "Element 0", and shows which asset is assigned
// without having to expand anything.
//
// WHY A DRAWER AND NOT A RENAME:
//
// Unity does substitute an array element's label for the value
// of a field - but only for a STRING field, conventionally
// called "name", declared first. CrybtSoundTable.Entry keys on
// a CrybtSound enum, which that feature does not look at, so
// renaming the field would have changed nothing except to
// orphan the enum value in every row of an already-authored
// asset. Unity YAML is name-keyed.
//
// Keeping the enum matters more than the label: it is what
// makes a missing row a compile-time-ish mistake rather than a
// misspelt string nobody notices.
//
// NOT A MIGRATION TOOL:
//
// The other scripts in this folder exist to be run once and
// deleted (see docs/code-index.md section 9). This one is
// permanent - it is how the table is edited.
// =============================================================

[CustomPropertyDrawer(typeof(CrybtSoundTable.Entry))]
public class CrybtSoundEntryDrawer : PropertyDrawer
{
    // The sound asset shares the header line with the label, so
    // a collapsed table still shows every assignment. Expanding
    // a row is only needed to change which sound it keys on.
    private const float LabelShareOfWidth = 0.4f;

    private const float MinLabelWidth = 80f;

    private const float MaxLabelWidth = 220f;


    // =========================================================
    // HEIGHT
    // =========================================================

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded)
        {
            return line;
        }

        return
            (line * 2f)
            + EditorGUIUtility.standardVerticalSpacing;
    }


    // =========================================================
    // DRAW
    // =========================================================

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        SerializedProperty id =
            property.FindPropertyRelative("id");

        SerializedProperty sound =
            property.FindPropertyRelative("sound");

        // If either field is ever renamed, fall back to the
        // default drawing rather than silently showing an
        // empty row.
        if (id == null || sound == null)
        {
            EditorGUI.PropertyField(position, property, label, true);

            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        float line = EditorGUIUtility.singleLineHeight;

        float labelWidth =
            Mathf.Clamp(
                position.width * LabelShareOfWidth,
                MinLabelWidth,
                MaxLabelWidth
            );

        Rect foldoutRect =
            new Rect(position.x, position.y, labelWidth, line);

        Rect soundRect =
            new Rect(
                position.x + labelWidth,
                position.y,
                position.width - labelWidth,
                line
            );

        property.isExpanded =
            EditorGUI.Foldout(
                foldoutRect,
                property.isExpanded,
                DisplayName(id),
                true
            );

        EditorGUI.PropertyField(soundRect, sound, GUIContent.none);

        if (property.isExpanded)
        {
            Rect idRect =
                new Rect(
                    position.x,
                    position.y
                        + line
                        + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    line
                );

            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(idRect, id);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }


    // =========================================================
    // LABEL
    // =========================================================
    //
    // enumDisplayNames is already nicified by Unity, so
    // CrybtSound.CardHover arrives as "Card Hover" with no
    // string work here.

    private static string DisplayName(SerializedProperty id)
    {
        if (id.hasMultipleDifferentValues)
        {
            return "—";
        }

        string[] names = id.enumDisplayNames;

        int index = id.enumValueIndex;

        if (names == null || index < 0 || index >= names.Length)
        {
            return "(unset)";
        }

        return names[index];
    }
}
