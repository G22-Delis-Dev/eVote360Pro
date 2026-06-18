namespace eVote360Pro.Application.DTOs;

/// <summary>DTO genérico para llenar dropdowns/SelectList en la capa Web.</summary>
public class DropdownItemDto
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
}
