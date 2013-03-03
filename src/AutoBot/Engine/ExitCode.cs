﻿using System.Collections;

namespace AutoBot.Cmd
{

    public enum ExitCode : int
    {
        Success = 0,
        CommandLineError = 1,
        MissingResourceFile = 2,
        UnknownError = 10
    }

}
