## Scoring System Planning

### combo / multiplier system

While working on the scoring module, I initially planned to implement a combo multiplier alongside the base score calculation.

After setting up the scoring flow, I realized the combo system is more of an enhancement than a core requirement. Since the assignment focuses on the fundamental gameplay loop, I decided to postpone this feature and keep the scoring logic simple for now.

### Decision

* Implement basic score calculation first.
* Keep the scoring code flexible so a combo/multiplier can be added later without major refactoring.

### Notes

The current scoring structure already separates score calculation from score display.

