﻿namespace DevsuCustomer.Api.Features;

public class GetCustomerResult
{
    public Guid ClienteId { get; set; }
    public string Contrasena { get; set; }
    public string Identificacion { get; set; }
    public string Nombre { get; set; }
    public string Genero { get; set; }
    public int Edad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public bool Estado { get; set; }
}