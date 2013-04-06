using System;
using System.Management.Automation.Host;

namespace AutoBot.Agents.PowerShell
{

    /// <summary>
    /// Defines the low-level host functionality, such as read and write actions,
    /// that a host application can implement to support cmdlets that perform
    /// character-mode interaction with the user. 
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.host.pshostrawuserinterface%28v=vs.85%29.aspx
    /// </remarks>
    internal sealed class RawUserInterface : PSHostRawUserInterface
    {

        #region Fields

        private Size m_BufferSize = new Size(80, 25);
        private ConsoleColor m_BackgroundColor = ConsoleColor.Black;
        private ConsoleColor m_ForegroundColor = ConsoleColor.White;
        private Coordinates m_CursorPosition = new Coordinates(0, 0);
        private int m_CursorSize = 1;

        #endregion

        #region PSHostRawUserInterface Members

        /// <summary>
        /// Gets or sets the background color of the displayed text.
        /// </summary>
        public override ConsoleColor BackgroundColor
        {
            get
            {
                return m_BackgroundColor;
            }
            set
            {
                m_BackgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the current size of the screen buffer.
        /// </summary>
        public override Size BufferSize
        {
            get
            {
                return m_BufferSize;
            }
            set
            {
                m_BufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the cursor position in the screen buffer.
        /// </summary>
        public override Coordinates CursorPosition
        {
            get
            {
                return m_CursorPosition;
            }
            set
            {
                m_CursorPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the cursor size as a percentage of a buffer cell.
        /// </summary>
        public override int CursorSize
        {
            get
            {
                return m_CursorSize;
            }
            set
            {
                m_CursorSize = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, flushes the input buffer.
        /// All input currently in the buffer is discarded. 
        /// </summary>
        public override void FlushInputBuffer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the foreground color of the displayed text.
        /// </summary>
        public override ConsoleColor ForegroundColor
        {
            get
            {
                return m_ForegroundColor;
            }
            set
            {
                m_ForegroundColor = value;
            }
        }

        /// <summary>
        /// Retrieves a rectangular region of the screen buffer.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value that indicates whether the user has pressed a key.
        /// </summary>
        public override bool KeyAvailable
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Determines the number of buffer cells occupied by a character.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override int LengthInBufferCells(char source)
        {
            return base.LengthInBufferCells(source);
        }

        /// <summary>
        /// Determines the number of buffer cells occupied by a string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override int LengthInBufferCells(string source)
        {
            return base.LengthInBufferCells(source);
        }

        /// <summary>
        /// Determines the number of buffer cells occupied by a portion of a specified string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override int LengthInBufferCells(string source, int offset)
        {
            return base.LengthInBufferCells(source, offset);
        }

        /// <summary>
        /// Gets the dimensions of the largest window that could be rendered in
        /// the current display, if the buffer was at the least that large.
        /// </summary>
        public override Size MaxPhysicalWindowSize
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the size of the largest window possible for the current buffer,
        /// current font, and current display hardware.
        /// </summary>
        public override Size MaxWindowSize
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Reads a key from the keyboard device. The variants of this method can
        /// read the key with or without required keystroke options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Crops a region of the screen buffer.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="clip"></param>
        /// <param name="fill"></param>
        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies an array of buffer cells into the screen buffer at a specified location.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="fill"></param>
        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies a given character, foreground color, and background color to a region of the screen buffer.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="contents"></param>
        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the position, in characters, of the view window relative to the screen buffer.
        /// </summary>
        public override Coordinates WindowPosition
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the current size of the view window.
        /// </summary>
        public override Size WindowSize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the title bar text of the current view window.
        /// </summary>
        public override string WindowTitle
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

    }

}
