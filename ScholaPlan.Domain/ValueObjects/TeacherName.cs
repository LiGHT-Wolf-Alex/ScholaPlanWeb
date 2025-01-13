﻿using System.Security.Cryptography;

namespace ScholaPlan.Domain.ValueObjects;

public class TeacherName
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public TeacherName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Имя и фамилия обязательны к заполнению");
        }

        FirstName = firstName;
        LastName = lastName;
    }
}