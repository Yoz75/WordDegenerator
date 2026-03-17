# What the hell is it?

**Word Degenerator is a pseudo-text generator** that mimics real text provided by the user. For example, given the input text `This is an example of the Word Degenerator program. The longer the input, the more realistic the output is. Try using entire books to achieve the best results. Changing the settings of Word Degenerator may also improve results!` Word Degenerator might generate `or progentic to also inger the longinputputput, to an examprove of Wor eneratore of Wor may achieve of Wor usinginginging may also improgeneramore longinput entingimpro`. Note that this text **was generated in Word Degenerator**, but probably you will get other result, so try experementing with the settings. Usually, longer texts produce more natural output.<br>
<img width="1910" height="1014" alt="изображение" src="https://github.com/user-attachments/assets/ee9ef4a2-3383-448f-b2f0-5f1470abc94d" />
By the way, Word Degenerator supports both Russian and English languages!

# How to run it
Sorry, but Word Degenerator supports **only** Windows 10 & 11 :(<br>
Download the latest release and unzip it somewhere. Inside the bin directory you'll find MANY files. Search for "WordDegenerator.exe" and run it.

# How to use it
Word Degenerator has an [ai generated documentation](https://github.com/Yoz75/WordDegenerator/blob/main/Docs/Documentation_en_user.md) (sorry about that) where all settings described. Also, there is a tip ontop of each setting or field when you hold your pointer on it (these ones are made by human).<br>
<br>
Short tokenizers tip:
* `BlockTokenizer` is fast but dumb sometimes
* `SeparatorTokenizer` works like T9. This one doesn't make "new words" (if you select space as separator)
* `RandomBlockTOkenizer` same as `BlockTokenizer`, but uses random sized blocks for better output
* `SmartTokenizer` tokenizer that uses [BPE-like](https://en.wikipedia.org/wiki/Byte-pair_encoding) algorythm, but very slow on long texts. Use at least 1000 iterations.
