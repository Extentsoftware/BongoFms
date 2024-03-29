﻿namespace Bongo.Domain.Models
{
    public class SprintCore
    {
        public string SprintName { get; set; } = default!;
        public DateTime? DateStarted { get; set; }
        public DateTime? DateEnded { get; set; }
        public DateTime DateCreated { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class SprintCoreId : SprintCore
    {
        public Guid Id { get; set; }
    }

    public class Sprint : SprintCoreId
    {
        public SprintStateDefinition[] States { get; set; } = default!;
        public SprintTask[] Tasks { get; set; } = default!;
    }
}
