# The Crybt — Scoring Rules

Reference notes for how encounters are scored in the Crybt card game, based on
[`CrybtManager.cs`](../Assets/Scripts/CrybtManager/CrybtManager.cs) and
[`Card.cs`](../Assets/Scripts/CrybtManager/Card.cs).

## Card values

Every card has three independent properties: **value** (used for fifteens),
**rank** (used for pairs and runs), and **suit** (used for flushes).

| Card | Value | Rank |
|---|---|---|
| Ace | **1** | 1 |
| 2 – 10 | Same as face (2 – 10) | Same as face (2 – 10) |
| Jack | 10 | 11 |
| Queen | 10 | 12 |
| King | 10 | 13 |

An Ace is **not** treated as 11 or as a high card anywhere in scoring. It is
always worth 1 point toward a fifteen, and its rank of 1 puts it at the low
end of a run, directly below a 2. There is no wraparound, so a King, Ace, 2
grouping does not form a run.

Jack, Queen, and King all collapse to a value of 10 for fifteens, but keep
their real rank (11, 12, 13) for pairs and runs.

## Combinations

The exact cards selected must themselves form the combination — the game does
not search a selection for a smaller hidden combination inside it.

| Combination | Requirement | Points |
|---|---|---|
| Fifteen | Selected cards total exactly 15 | 2 |
| Pair | 2 cards, same rank | 2 |
| Three of a kind | 3 cards, same rank | 6 |
| Four of a kind | 4 cards, same rank | 12 |
| Run | 3+ cards, consecutive rank | 1 per card |
| Four-card flush | Exactly 4 cards, same suit | 4 |
| Five-card flush | Exactly 5 cards, same suit | 5 |

Rules stack on the same selection — a five-card straight flush scores both the
run (5) and the flush (5). Each unique group of cards can only be confirmed
for points once per encounter; re-selecting the same set scores nothing.

## Offering scoring (end of crawl)

Unlike a manually confirmed combination, the Offering pile is scored by
searching every subset for fifteens:

- Every subset of the Offering that totals 15 pays 2 points.
- Every pair among the Offering cards pays 2 points.
- Every card in the Graveyard pays 1 point.

These points are spent in the Boon Shop and do not carry over between crawls.
