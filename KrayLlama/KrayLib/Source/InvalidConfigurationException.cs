// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

namespace KrayLib;

/// <summary>
/// Исключение, возбуждаемое при ошибочной конфигурации.
/// </summary>
public sealed class InvalidConfigurationException
    : ApplicationException
{
    #region Constructors

    public InvalidConfigurationException
        (
            string? message
        )
        : base (message)
    {
        // пустое тело конструктора
    }

    public InvalidConfigurationException
        (
            string? message,
            Exception? innerException
        )
        : base (message, innerException)
    {
        // пустое тело конструктора
    }

    #endregion
}
