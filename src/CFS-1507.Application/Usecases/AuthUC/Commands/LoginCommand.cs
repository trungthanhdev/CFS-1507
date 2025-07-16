using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using CFS_1507.Contract.DTOs.AuthDto.Response;
using MediatR;

namespace CFS_1507.Application.Usecases.AuthUC.Commands
{
    public class LoginCommand(ReqLoginDto arg) : IRequest<ResponseAuthDto>
    {
        public ReqLoginDto Arg = arg;
    }
    public class LoginCommandHandler(

    ) : IRequestHandler<LoginCommand, ResponseAuthDto>
    {
        public Task<ResponseAuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}