using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public int StatusCode { get; }
        public string? ErrorMessage { get; }

        private Result(bool isSuccess, int? statusCode, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true, null, null);
        public static Result Fail(int statusCode, string errorMessage) => new(false, statusCode, errorMessage);
    }

    public class Result<T>
    {
        public T? Value { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public int StatusCode { get; }
        public string? ErrorMessage { get; }

        private Result(bool isSuccess, T? value, int? statusCode, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(true, value, null, null);
        public static Result<T> Fail(int statusCode, string errorMessage) => new(false, default, statusCode, errorMessage);
    }
}
