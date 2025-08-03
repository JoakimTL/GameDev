Production is turning one set of resources into another set of resources intentionally.

Among the used resources are work hours and materials. Outputs usually come in the form of new materials.

There are some sectors which produce conveniences rather than materials. I don't know if the game will cover those in any way. The concept is fun, but linking it to profits for a building will be hard.

Production of materials work this way:
A building has workers which spend time of their days performing recipes available to that building if the prerequisites for that recipe is available.
The recipes are prioritized or barred, which means some production recipes are preferred over others, but if the prerequisites are not met another lower priority recipe will be chosen. Barred recipes are not eligible for production. The Empire chooses which recipes are prioritized and barred (at least let's work like that for now. Capitalism and other things should probably change that.)

# Output cycles
Workers have a set number of output cycles per day, defined by the number of hours workers are required to work and the exhaustion level of the building. The number of hours per day has a diminishing returns when it comes out output cycles, to represent exhaustion as the day goes on. Finding the balance between working hours and worker productivity as well as worker satisfaction and wages ends up being a crucial factor for production.

Building output exhaustion is the characterization of workers being overworked, at least in most cases. When workers have too little rest time per day, the following days end up being less productive.

Below are the formulas for finding the number of output cycles per day per worker for a given number of hours and exhaustion level.
Here $t_0$ describes the number of hours a worker works at 100% productivity, $t_1$ is the number of hours at which a worker can be considered "spent" and working past this gives no meaningful value. $r$ is the number of hours of rest workers need to not become exhausted, at least to the point of affecting productivity. $g$ is the exhaustion debt accumulation factor, simply meaning each hour accounts for $g$ number of hours of exhaustion debt.
$$t_0=4,\quad t_1=18,\quad r=10,\quad g=0.25$$
$$t_d=\frac{(t_1-t_0)}{2},\quad t_i=t_d+t_0,\quad k=\frac{e}{t_d}$$
$$E'(t)=\frac{1}{1+e^{k(t-t_i)}}$$
$$E(t)=t_i-\frac{1}{k}\ln(1+e^{-k(t-t_i)})$$
$$f_{i+1}=clamp(f_{i}+(t+r-24) \cdot g,0,24)$$
$$C(t,f_i)=E(t)-E(f_i)$$
Aside from keeping track of $f_i$ per building, the way to find the number of output cycles per worker per day for a building you simply need to find $C(t,f_i)$ where $t$ is the number of work hours per day and $f_i$ is the current exhaustion debt.

For an illustration of the non-linear nature of $E(t)$ take a look at the picture below where the red line is the output cycle for each hour and the blue line is the accumulated output cycles for that number of hours that day.
![[Exhaustion Antiderivative.png]]
# Recipes
Recipes are defined ways of producing a set of materials using another set of materials. A concrete example would be forging steel into a sword. The process uses steel ingots and returns a steel sword which can then either be used in other recipes if they exist or be stockpiled for other uses, such as warfare in this example.

Buildings have a set of recipes it can use. The recipes are prioritized by the Empire and some are disallowed. Only recipes which fulfil the recipe prerequisites are eligible for production. This means recipes using unknown technologies or materials which are unavailable can't be used by the building. In the case of unknown technologies the best approach would be for the recipe to not be available for viewing in the UI either.