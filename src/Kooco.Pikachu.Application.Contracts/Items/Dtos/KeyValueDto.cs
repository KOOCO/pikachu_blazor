using System;

namespace Kooco.Pikachu.Items.Dtos
{
    public class KeyValueDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ZhName { get; set; }

        public KeyValueDto()
        {
            
        }

        public KeyValueDto(
            Guid id,
            string name,
            string? zhName
            )
        {
            Id = id;
            Name = name;
            ZhName = zhName;
        }
    }
}
