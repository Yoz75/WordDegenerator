# What is it?
**Word Degenerator is a pseudo-text generator** that mimics real text provided by the user. For example, given the input text `This is an example of the Word Degenerator program. The longer the input, the more realistic the output is. Try using entire books to achieve the best results. Changing the settings of Word Degenerator may also improve results!` Word Degenerator might generate `or progentic to also inger the longinputputput, to an examprove of Wor eneratore of Wor may achieve of Wor usinginginging may also improgeneramore longinput entingimpro`. Usually, longer texts produce more natural output.<br>
<img width="1919" height="1030" src="https://github.com/user-attachments/assets/bb13d25a-d297-4b1a-a7bd-a3231b8be79f" />

By the way, Word Degenerator supports both Russian and English languages!

# How to run it
Word Degenerator supports **only** Windows 10.1509 & 11<br>
You can build the solution by yourself or download the latest release and unzip it. Inside the bin directory you'll find WordDegenerator.exe

# How to use it
Word Degenerator has an [ai generated documentation](https://github.com/Yoz75/WordDegenerator/blob/main/Docs/Documentation_en_user.md) (sorry about that) where all settings described. Also, there is a tip on top of each setting or field when you hold your pointer on it (these ones are made by human).<br>
<br>
Short tokenizers tip:
* `BlockTokenizer` is fast but dumb sometimes
* `SeparatorTokenizer` works like T9. This one doesn't make "new words" (if you select space as separator)
* `RandomBlockTOkenizer` same as `BlockTokenizer`, but uses random sized blocks for better output
* `SmartTokenizer` tokenizer that uses [BPE-like](https://en.wikipedia.org/wiki/Byte-pair_encoding) algorithm, but very slow on long texts. Use at least 1000 iterations.

# Known bugs:
 * TopK and output length of Default Generator are 1 by default