**Tags**: #culture-aspect

A culture may like another culture, or if the polarity is negative hold a grudge. These are usually formed from long interactions between nations where cultures in each nation end up holding an opinion of another culture from that nation based on their long term interaction pattern. If there are constant wars the cultures between the countries may be wary of each other, but if there is longstanding alliances and trade relations the cultures may respect each other.

This is a crucial aspect of cultures, as a hostile culture can be really hard to annex.

So the actual opinion value is found by $O=S \cdot P$ where $S$ is saliency and $P$ is polarity. This aspect is the truth of a cultures opinion of another.

The value has an equilibrium value calculated based on the cultures similarities and their closeness in the culture tree.

The equilibrium value comes in 2 parts:
- A constant value calculated based on the immediate game state. If this value changes then the opinion value will slowly drift.
- An event based system which adds or subtracts to the equilibrium point which causes drift. The events have half lives and once they are negligible they are removed from the event list.

When equilibrium drifts are the only factors to the opinion then the polarity is changed first, then saliency. When $O$ matches the constant equilibrium and there are no ongoing events drifting this aspect will be removed for memory efficiency. When the constant value calculation changes then the opinion must drift again and this aspect reinstated. The game will act as if this aspect always exists.

When an external factor applies cultural pressure to change the opinion then the value slowly drifts toward equilibrium again. The pressure applies tension if the rigidity of the aspect is high.