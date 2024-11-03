using System.ComponentModel.DataAnnotations;

namespace TaskBreakerApi.Models {

    public enum OElementType {
        Idea,        // Общая идея или концепция задачи
        Subtask,     // Подзадача, относящаяся к основной задаче
        Step,        // Конкретный шаг к решению задачи
        Requirement, // Требование, которое должно быть выполнено
        Challenge,   // Возможные проблемы или препятствия
        Outcome      // Ожидаемый результат выполнения задачи
    }

    public class OElement {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ObjectiveId { get; set; } = -1;
        [Required] 
        public int ConnectedTo { get; set; } = -1;
        [Required]
        public OElementType Type { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
