using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{



    [Serializable] public class InvalidAppAccessKeyOrSecretException : Exception {public InvalidAppAccessKeyOrSecretException() { }}

    [Serializable] public class AlreadyExistException : Exception { public AlreadyExistException() { } }
    [Serializable] public class DuplicateValuesAreNotAllowedException : Exception { public DuplicateValuesAreNotAllowedException() { } }


    [Serializable] public class InvalidTokenException : Exception { public InvalidTokenException() { } }

    [Serializable] public class UnauthorizedException : Exception { public UnauthorizedException() { } }

    [Serializable] public class IncorrectPasswordException : Exception { public IncorrectPasswordException() { } }

    [Serializable] public class InvalidParametersException : Exception { public InvalidParametersException() { } }

    [Serializable] public class DatabaseConnectionFailedException : Exception { public DatabaseConnectionFailedException() { } }




    [Serializable] public class NotFoundException : Exception { public NotFoundException() { } }

    [Serializable] public class UserNotFoundException : Exception { public UserNotFoundException() { } }
    [Serializable] public class RoleNotFoundException : Exception { public RoleNotFoundException() { } }

    [Serializable] public class AppNotFoundException : Exception { public AppNotFoundException() { } }
    [Serializable] public class AppAccessNotFoundException : Exception { public AppAccessNotFoundException() { } }

    [Serializable] public class AppPermissionTypeNotFoundException : Exception { public AppPermissionTypeNotFoundException() { } }









}
