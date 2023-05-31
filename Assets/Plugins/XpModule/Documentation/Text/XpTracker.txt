# Presentation  
  Component allowing to store mutiples experiences tracks.  
  
<br>
<br>
  
# Required Components  
 - None
  
<br>
<br>
  
# Set Up  
 - 1 : Add the XpTracker component to your prefab  
 - 2 : Define Experience list  
 - 3 (optional) : listen for EventExperienceUpdated and/or EventLevelUpdated events triggered by Experiences scripts.
 - 4 : Call XpTracker.Grant() in your own logic  
  
<br>
<br>
  
# Properties  
  
  ## Experiences : List<Experience>
  Skill tree as a list of Experience class.
  
<br>
<br>
  
# Methods
  ## Get(string name)
  Return Experience object associated with given name. If no Experience match given name, return null.

  ## Grant(string name, float value)
  Grant ${value} number of experience points to Experience stored at ${name}.
  
<br>
<br>


