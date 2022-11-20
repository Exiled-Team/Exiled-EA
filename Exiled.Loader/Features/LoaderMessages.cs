// -----------------------------------------------------------------------
// <copyright file="LoaderMessages.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features
{
    using System;

    /// <summary>
    /// A class that contains the different EXILED loader messages.
    /// </summary>
    public static class LoaderMessages
    {
        /// <summary>
        /// Gets the default loader message.
        /// </summary>
        public static string Default
        {
            get => @"
   ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
 ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
  ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
  ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
  ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
                                  ▀                                 ";
        }

        /// <summary>
        /// Gets the easter egg loader message.
        /// </summary>
        public static string EasterEgg
        {
            get => @"
   ▄████████    ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  ███    ███   ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  ███    █▀    ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
  ███         ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
▀███████████ ▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
         ███   ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
   ▄█    ███   ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
 ▄████████▀    ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
                                                                                ";
        }

        /// <summary>
        /// Gets the christmas loader message.
        /// </summary>
        public static string Christmas
        {
            get => @"
       __
    .-'  |
   /   <\|        ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  /     \'       ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  |_.- o-o       ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
  / C  -._)\    ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
 /',        |  ▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
|   `-,_,__,'    ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
(,,)====[_]=|    ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
  '.   ____/     ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
   | -|-|_
   |____)_)";
        }

        /// <summary>
        /// Gets the halloween loader message.
        /// </summary>
        public static string Halloween
        {
            get => @"
@@@@@@@@  @@@  @@@  @@@  @@@       @@@@@@@@  @@@@@@@
@@@@@@@@  @@@  @@@  @@@  @@@       @@@@@@@@  @@@@@@@@
@@!       @@!  !@@  @@!  @@!       @@!       @@!  @@@
!@!       !@!  @!!  !@!  !@!       !@!       !@!  @!@
@!!!:!     !@@!@!   !!@  @!!       @!!!:!    @!@  !@!
!!!!!:      @!!!    !!!  !!!       !!!!!:    !@!  !!!
!!:        !: :!!   !!:  !!:       !!:       !!:  !!!
:!:       :!:  !:!  :!:   :!:      :!:       :!:  !:!
 :: ::::   ::  :::   ::   :: ::::   :: ::::   :::: ::
: :: ::    :   ::   :    : :: : :  : :: ::   :: :  :
                                                       ";
        }

        /// <summary>
        /// Gets the loader message according to the actual month.
        /// </summary>
        /// <returns>The correspondent loader message.</returns>
        public static string GetMessage()
        {
            if (Loader.Random.NextDouble() <= 0.069)
                return EasterEgg;

            return DateTime.Today.Month switch
            {
                12 => Christmas,
                10 => Halloween,
                _ => Default,
            };
        }
    }
}