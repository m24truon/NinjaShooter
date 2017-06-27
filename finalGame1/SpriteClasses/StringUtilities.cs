using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriteClasses
{
    public class StringUtilities
    {
        /// <summary>
        /// For use with graphics. Used to change name of image to next image in animation sequence 
        /// or back to 0 when all images have been shown.
        /// replaces the last two digits of a filename with another two digits.
        /// Assumes the filename ends with a two digit image number. 
        /// For this to work, all animations must follow this naming convention.
        /// </summary>
        /// <param name="filename">original filename, no extension</param>
        /// <param name="newNumber">number to replace last two digits with</param>
        /// <returns></returns>
        public static string NextImageName(string filename, int newNumber)
        {
            //if number < 10, have to add a 0 to the front of it so last two characters get replaced
            if (newNumber < 10)
            {
                filename = ReplaceOnce(filename, "0" + newNumber, filename.Length - 2);
            }
            //if not, just use number as is since it's two digits
            else
            {
                filename = ReplaceOnce(filename, newNumber.ToString(), filename.Length - 2);
            }
            return filename;
        }

        /// <summary>
        /// Works with a string, replacing one occurence of a string 
        /// at a specified position with another string.
        /// Counting of positions starts with the first character as 0. 
        /// </summary>
        /// <param name="word">the string you are working with</param>
        /// <param name="character">the string you want to put into it</param>
        /// <param name="position">the position the string needs to start</param>
        /// <returns>The modified string</returns>
        public static string ReplaceOnce(string word, string characters, int position)
        {
            word = word.Remove(position, characters.Length);
            word = word.Insert(position, characters);
            return word;
        }

        /// <summary>
        /// Gets and returns the file extension from the end of a filename.
        /// Doesn't matter how long the extension is because it returns the
        /// letters from the last . to the end 
        /// </summary>
        /// <param name="name">filename</param>
        /// <returns>the extension from the . to the end</returns>   
        public static string GetLastName(string name)
        {
            string result = "";
            int posn = name.LastIndexOf(' ');
            if (posn >= 0) result = name.Substring(posn + 1);
            return result;
        }
    }
}
     