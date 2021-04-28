using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create your validator class and inherit TMPro.TMP_InputValidator 
/// Note that this is a ScriptableObject, so you'll have to create an instance of it via the Assets -> Create -> Input Field Validator 
/// </summary>
[CreateAssetMenu(fileName = "Integer Validator", menuName = "Integer Validator")]
public class IntegerValidator : TMPro.TMP_InputValidator
{
    /// <summary>
    /// Override Validate method to implement your own validation
    /// </summary>
    /// <param name="text">This is a reference pointer to the actual text in the input field; changes made to this text argument will also result in changes made to text shown in the input field</param>
    /// <param name="pos">This is a reference pointer to the input field's text insertion index position (your blinking caret cursor); changing this value will also change the index of the input field's insertion position</param>
    /// <param name="ch">This is the character being typed into the input field</param>
    /// <returns>Return the character you'd allow into </returns>
    public override char Validate(ref string text, ref int pos, char ch)
    {
        Debug.Log($"Text = {text}; pos = {pos}; chr = {ch}");
        // If the typed character is a number, insert it into the text argument at the text insertion position (pos argument)
        if (char.IsNumber(ch) && text.Length < 14)
        {
            // Insert the character at the given position if we're working in the Unity Editor
#if UNITY_EDITOR
            text = text.Insert(pos, ch.ToString());
#endif
            // Increment the insertion point by 1
            pos++;
            // Convert the text to a number
            long number = ConvertToInt(text);
            // If the number is greater than 9999999999 / 11 characters or longer
            // Then reparse the number string but trimmed to 10 characters
            if (number > 9999999999)
            {
                number = long.Parse(number.ToString().Substring(0, 10));
            }

            // Format the string incrementally by the character count of our number
            // Format: (XXX) XXX-XXXX
            switch (number.ToString().Length)
            {
                case 1:
                    text = System.String.Format("{0:(#}", number);
                    break;
                case 2:
                    text = System.String.Format("{0:(##}", number);
                    break;
                case 3:
                    text = System.String.Format("{0:(###) }", number);
                    break;
                case 4:
                    text = System.String.Format("{0:(###) #}", number);
                    break;
                case 5:
                    text = System.String.Format("{0:(###) ##}", number);
                    break;
                case 6:
                    text = System.String.Format("{0:(###) ###-}", number);
                    break;
                case 7:
                    text = System.String.Format("{0:(###) ###-#}", number);
                    break;
                case 8:
                    text = System.String.Format("{0:(###) ###-##}", number);
                    break;
                case 9:
                    text = System.String.Format("{0:(###) ###-###}", number);
                    break;
                case 10:
                    text = System.String.Format("{0:(###) ###-####}", number);
                    break;
                default:

                    return '\0';
            }


            // Increment the text insertion position by 1 in the following positions
            // (
            // (XXX)_XXX-
            if (pos == 1 || pos == 9)
            {
                pos++;
            }
            // Increment the text insertion position by 2 in the following positions
            // (XXX)
            // (XXX)_
            else if (pos == 4 || pos == 5)
            {
                pos += 2;
            }
            return ch;

        }
        // If the character is not a number, return null
        else
        {
            return '\0';
        }
    }

    /// <summary>
    /// Converts a string input into a long integer
    /// </summary>
    /// <param name="inputText">Input text</param>
    /// <returns>Returns the number of the text</returns>
    long ConvertToInt(string inputText)
    {
        // Create a string builder to cache our number  characters
        System.Text.StringBuilder number = new System.Text.StringBuilder();
        // Iterate through each character in the input text
        // If the character found is a digit, append the digit to our string builder
        foreach (char character in inputText)
        {
            if (char.IsDigit(character))
            {
                number.Append(character);
            }
        }
        // Return the numbered string
        return long.Parse(number.ToString());
    }
}