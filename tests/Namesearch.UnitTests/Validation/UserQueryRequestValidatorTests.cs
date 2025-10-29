using FluentAssertions;
using Namesearch.Application.Contracts;
using Namesearch.Application.Validation;
using Xunit;

namespace Namesearch.UnitTests.Validation;

public class UserQueryRequestValidatorTests
{
    [Fact]
    public void Should_fail_when_query_is_empty()
    {
        var validator = new UserQueryRequestValidator();
        var result = validator.Validate(new UserQueryRequest { Query = "" });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_pass_for_basic_valid_input()
    {
        var validator = new UserQueryRequestValidator();
        var result = validator.Validate(new UserQueryRequest { Query = "John", Page = 1, PageSize = 5 });
        result.IsValid.Should().BeTrue();
    }
}
