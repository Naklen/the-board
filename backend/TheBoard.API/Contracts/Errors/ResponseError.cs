using FluentResults;

namespace TheBoard.API.Contracts.Errors;

public record ResponseErrors(List<IError> Errors);
