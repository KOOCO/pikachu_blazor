using System;

namespace Kooco.Pikachu.Items.Dtos
{
    public class KeyValueDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public KeyValueDto()
        {
            
        }

        public KeyValueDto(
            Guid id,
            string name
            )
        {
            Id = id;
            Name = name;
        }
    }
}
