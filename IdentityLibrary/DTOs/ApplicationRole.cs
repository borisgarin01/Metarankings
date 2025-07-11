using Microsoft.AspNetCore.Identity;
using Npgsql.PostgresTypes;
using System.ComponentModel.DataAnnotations;

namespace IdentityLibrary.DTOs;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole(string name) : base(name)
    {

    }
}
