using System;


namespace Backend_CRUD.Domain.DTOs.Responses
{
    public class PaginatedResponseDTO<T> : ResponseDTO<T>
    {
        public int Count { get; set; }
    }
}
