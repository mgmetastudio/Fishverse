# Presentation  
  Generic class that handle experience and level for any need.  
  
<br>
<br>

# Properties  
  
  ## Id : string
  Unique Id for this specific experience. Ex : "Woodworking", "Combat", "Singing", ...  

  ## Floors : List<float>
  Quantity of experience points to accumulate in order to
  reach next level.  
  
  Note : experience points does not reset once a level is reached, as result, no experience point is lost if the asset
  wins more experience than required to reach next level.
  
  ## Capped : Boolean
  Should script stop accumulating experience once maximum level is reached ?  
  
<br>
<br>
  
# Getters - Setters  
  
  ## Points : float
  Get : return points value
  Set : update points value and check if level is modified
  
<br>
<br>
  
# Methods
  
  ## GetFloor(int level)
  Return floor value for given level. If requested level does not exist, return 0.

  Because experience do not reset on level gain, you may want to use this method to calculate number of experience points required to reach next level :

  ```
  float experienceRequired = Xp.GetFloor(Xp.Level) - Xp.GetFloor(Xp.Level - 1);
  float experienceAcquired = Xp.Experience - Xp.GetFloor(Xp.Level - 1);
  ```
  
<br>
<br>
  
# Events  
  This component is casting events to notify  
  Experience and Level values modifications.  
  It is useful if you want to plug additional  
  components using theses informations like some  
  experience UI, or some graphicals effects.  
  
  ## EventLevelUpdated(Experience experience, int value)  
  Trigger when experience gain leads to a level modification.
  
  ## EventExperienceUpdated(Experience experience, float value)  
  Trigger when there is an experience gain.



