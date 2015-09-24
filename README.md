#LanguageTools

The purpose of this project is to create a set of Office plugins, that show the gender of German words while typing.
Currently, only gender information is supported.

*Planned improvements:*
- Also store the meaning of words
- Add word-infliction data. This could come from morphisto, who (still needs to be checked) provide their data freely
for users too.
*Unplanned improvements:*
- Support other languages. Shouldn't be so hard to do, since most of the code is language agnostic. Would have to split
parts of the Backend code in a separate module for German.

##DatabaseManager
This tool allows you to manage your word database. You can import Gender data from the dict.cc dictionary. Because of
the licensing terms on that dictionary, you need to manually download it and import it using the DatabaseManager.

##LanguageToolsWord
This is the Word addin. It shows a panel with the search results. Currently supports to search automatically on selecting
a word, or search manually.

*Planned improvements:*
- This project contains a Word addin that shows the genders of German words while working in Word.
- Currently, you need to select the word you want searched (moving around with the arrow-keys also works). Would be nice
if this also works while typing.

##LanguageToolsBackend
The backend library for the whole package. This library handles all database and German logic.
