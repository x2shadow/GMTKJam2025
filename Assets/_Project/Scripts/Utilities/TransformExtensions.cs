using UnityEngine;

/// <summary>
/// Набор расширений для удобной установки родителя с компенсацией масштаба,
/// чтобы объект всегда имел заданный мировой размер.
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// Переназначает родителя и одновременно компенсирует локальный масштаб так,
    /// чтобы мировой масштаб объекта соответствовал desiredWorldScale.
    /// </summary>
    /// <param name="child">Трансформ дочернего объекта.</param>
    /// <param name="newParent">Новый родитель.</param>
    /// <param name="desiredWorldScale">Желаемый мировой масштаб объекта.</param>
    /// <param name="worldPositionStays">Если true — сохраняет мировую позицию/ротацию при смене родителя.</param>
    public static void SetParentWithWorldScale(this Transform child, Transform newParent, Vector3 desiredWorldScale, bool worldPositionStays = true)
    {
        // Сохраняем мировую позицию/ротацию, если нужно
        child.SetParent(newParent, worldPositionStays);

        // Получаем во сколько растянут родитель
        Vector3 parentScale = newParent.lossyScale;

        // Вычисляем локальный масштаб, чтобы worldScale = parentScale * localScale
        child.localScale = new Vector3(
            desiredWorldScale.x / parentScale.x,
            desiredWorldScale.y / parentScale.y,
            desiredWorldScale.z / parentScale.z
        );
    }
}
