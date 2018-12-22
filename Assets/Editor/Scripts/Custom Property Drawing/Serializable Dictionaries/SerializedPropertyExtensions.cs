using UnityEditor;

namespace CustomPropertyDrawing.SerializedDictionaries
{
    public static class SerializedPropertyExtension
    {
        public static int GetObjectCode(this SerializedProperty p) => p.propertyPath.GetHashCode() ^ p.serializedObject.GetHashCode();

        public static bool EqualBasics(SerializedProperty left, SerializedProperty right)
        {
            if (left.propertyType != right.propertyType)
                return false;

            switch (left.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return left.boolValue == right.boolValue;

                case SerializedPropertyType.Integer:
                    if (left.type == right.type)
                    {
                        if (left.type == "int") return left.intValue == right.intValue;
                        else return left.longValue == right.longValue;
                    }
                    else return false;

                case SerializedPropertyType.String:
                    return left.stringValue == right.stringValue;

                case SerializedPropertyType.ObjectReference:
                    return left.objectReferenceValue == right.objectReferenceValue;

                case SerializedPropertyType.Enum:
                    return left.enumValueIndex == right.enumValueIndex;

                case SerializedPropertyType.Float:
                    if (left.type == right.type)
                    {
                        if (left.type == "float") return left.floatValue == right.floatValue;
                        else return left.doubleValue == right.doubleValue;
                    }
                    else return false;

                case SerializedPropertyType.Color:
                    return left.colorValue == right.colorValue;

                case SerializedPropertyType.LayerMask:
                    return left.intValue == right.intValue;

                case SerializedPropertyType.Vector2:
                    return left.vector2Value == right.vector2Value;
                case SerializedPropertyType.Vector3:
                    return left.vector3Value == right.vector3Value;

                case SerializedPropertyType.Vector4:
                    return left.vector4Value == right.vector4Value;

                case SerializedPropertyType.Rect:
                    return left.rectValue == right.rectValue;

                case SerializedPropertyType.ArraySize:
                    return left.arraySize == right.arraySize;

                case SerializedPropertyType.Character:
                    return left.intValue == right.intValue;

                case SerializedPropertyType.Bounds:
                    return left.boundsValue == right.boundsValue;

                case SerializedPropertyType.Quaternion:
                    return left.quaternionValue == right.quaternionValue;

                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Gradient:
                default:
                    return false;
            }
        }

        public static void CopyBasics(SerializedProperty source, SerializedProperty target)
        {
            if (source.propertyType != target.propertyType)
                return;

            switch (source.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (source.type == target.type)
                    {
                        if (source.type == "int") target.intValue = source.intValue;
                        else target.longValue = source.longValue;
                    }
                    break;

                case SerializedPropertyType.String:
                    target.stringValue = source.stringValue;
                    break;

                case SerializedPropertyType.ObjectReference:
                    target.objectReferenceValue = source.objectReferenceValue;
                    break;

                case SerializedPropertyType.Enum:
                    target.enumValueIndex = source.enumValueIndex;
                    break;

                case SerializedPropertyType.Boolean:
                    target.boolValue = source.boolValue;
                    break;

                case SerializedPropertyType.Float:
                    if (source.type == target.type)
                    {
                        if (source.type == "float") target.floatValue = source.floatValue;
                        else target.doubleValue = source.doubleValue;
                    }
                    break;

                case SerializedPropertyType.Color:
                    target.colorValue = source.colorValue;
                    break;

                case SerializedPropertyType.LayerMask:
                    target.intValue = source.intValue;
                    break;

                case SerializedPropertyType.Vector2:
                    target.vector2Value = source.vector2Value;
                    break;

                case SerializedPropertyType.Vector3:
                    target.vector3Value = source.vector3Value;
                    break;

                case SerializedPropertyType.Vector4:
                    target.vector4Value = source.vector4Value;
                    break;

                case SerializedPropertyType.Rect:
                    target.rectValue = source.rectValue;
                    break;

                case SerializedPropertyType.ArraySize:
                    target.arraySize = source.arraySize;
                    break;

                case SerializedPropertyType.Character:
                    target.intValue = source.intValue;
                    break;

                case SerializedPropertyType.AnimationCurve:
                    target.animationCurveValue = source.animationCurveValue;
                    break;

                case SerializedPropertyType.Bounds:
                    target.boundsValue = source.boundsValue;
                    break;

                case SerializedPropertyType.Gradient:
                    // TODO?
                    break;

                case SerializedPropertyType.Quaternion:
                    target.quaternionValue = source.quaternionValue;
                    break;

                default:
                    {
                        if (source.hasChildren && target.hasChildren)
                        {
                            var sourceIterator = source.Copy();
                            var targetIterator = target.Copy();
                            while (true)
                            {
                                if (sourceIterator.propertyType == SerializedPropertyType.Generic)
                                {
                                    if (!sourceIterator.Next(true) || !targetIterator.Next(true))
                                        break;
                                }
                                else if (!sourceIterator.Next(false) || !targetIterator.Next(false))
                                {
                                    break;
                                }
                                else CopyBasics(sourceIterator, targetIterator);
                            }
                        }
                    }
                    break;
            }
        }
    }
}