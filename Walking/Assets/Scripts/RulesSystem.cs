﻿using AI.Fuzzy.Library;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RulesSystem
{
    public static void RulesForCrowdAvoidance(MamdaniFuzzySystem avoidanceSystem)
    {
        avoidanceSystem.Rules.Clear();
        //angle
        MamdaniFuzzyRule rule1 = avoidanceSystem.ParseRule("if (FrontDistance is Far) then (Angle is Zero)");

        MamdaniFuzzyRule rule2 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) and (RightDistance is Near) and (VeryRightDistance is Far) then (Angle is Negative)");
        MamdaniFuzzyRule rule3 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) and (RightDistance is Far) and (VeryRightDistance is Near) then (Angle is LittleNegative)");

        MamdaniFuzzyRule rule4 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is Positive)");
        MamdaniFuzzyRule rule5 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Far) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is LittlePositive)");

        MamdaniFuzzyRule rule6 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is Positive)");
        MamdaniFuzzyRule rule7 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is VeryPositive)");

        MamdaniFuzzyRule rule8 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Far) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is LittleNegative)");
        MamdaniFuzzyRule rule9 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is LittlePositive)");

        MamdaniFuzzyRule rule10 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) then (Angle is Negative)");
        MamdaniFuzzyRule rule11 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is Positive)");

        MamdaniFuzzyRule rule12 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is VeryNegative)");
        MamdaniFuzzyRule rule13 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is VeryPositive)");

        MamdaniFuzzyRule rule14 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is LittlePositive)");
        MamdaniFuzzyRule rule15 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is LittleNegative)");

        MamdaniFuzzyRule rule16 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is Zero)");
        //MamdaniFuzzyRule rule26 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (ColliderType is Static) and (RightDistance is extremely Near) and (VeryRightDistance is extremely Near) and (LeftDistance is extremely Near) and (VeryLeftDistance is extremely Near) then (Angle is TurnAround)");
        MamdaniFuzzyRule rule17 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) and (RightDistance is Far) and (VeryRightDistance is Far) then (Angle is Positive)");

        //speed
        MamdaniFuzzyRule rule18 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Stay) and ((TargetMovement is Walk) or (TargetMovement is Run)  or (TargetMovement is Sprint)) then (SpeedChange is Faster)");
        MamdaniFuzzyRule rule19 = avoidanceSystem.ParseRule("if (FrontDistance is Far) and (CurrentMovement is SlowWalk) and ((TargetMovement is Walk) or (TargetMovement is Run)  or (TargetMovement is Sprint)) then (SpeedChange is Faster)");
        MamdaniFuzzyRule rule20 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Walk) and ((TargetMovement is Run) or (TargetMovement is Sprint)) then (SpeedChange is Faster)");
        MamdaniFuzzyRule rule21 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Run) and ((TargetMovement is Sprint)) then (SpeedChange is Faster)");

        MamdaniFuzzyRule rule22 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Sprint) and (TargetMovement is Stay) then (SpeedChange is Slower)");
        MamdaniFuzzyRule rule23 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Sprint) and (TargetMovement is Walk) then (SpeedChange is Slower)");
        MamdaniFuzzyRule rule24 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Sprint) and (TargetMovement is Run) then (SpeedChange is Slower)");
        MamdaniFuzzyRule rule25 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Run) and (TargetMovement is Walk) then (SpeedChange is Slower)");
        MamdaniFuzzyRule rule26 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Run) and (TargetMovement is Stay) then (SpeedChange is Slower)");
        MamdaniFuzzyRule rule27 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Walk) and (TargetMovement is Stay) then (SpeedChange is Slower)");

        MamdaniFuzzyRule rule28 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and ((CurrentMovement is Walk) or (CurrentMovement is Run) or (CurrentMovement is Sprint)) then (SpeedChange is Slower)");
        MamdaniFuzzyRule rule29 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (CurrentMovement is Stay) then (SpeedChange is Faster)");


        

        avoidanceSystem.Rules.Add(rule1);
        avoidanceSystem.Rules.Add(rule2);
        avoidanceSystem.Rules.Add(rule3);
        avoidanceSystem.Rules.Add(rule4);
        avoidanceSystem.Rules.Add(rule5);
        avoidanceSystem.Rules.Add(rule6);
        avoidanceSystem.Rules.Add(rule7);
        avoidanceSystem.Rules.Add(rule8);
        avoidanceSystem.Rules.Add(rule9);
        avoidanceSystem.Rules.Add(rule10);
        avoidanceSystem.Rules.Add(rule11);
        avoidanceSystem.Rules.Add(rule12);
        avoidanceSystem.Rules.Add(rule13);
        avoidanceSystem.Rules.Add(rule14);
        avoidanceSystem.Rules.Add(rule15);
        avoidanceSystem.Rules.Add(rule16);
        avoidanceSystem.Rules.Add(rule17);
        avoidanceSystem.Rules.Add(rule18);
        avoidanceSystem.Rules.Add(rule19);
        avoidanceSystem.Rules.Add(rule20);
        avoidanceSystem.Rules.Add(rule21);
        avoidanceSystem.Rules.Add(rule22);
        avoidanceSystem.Rules.Add(rule23);
        avoidanceSystem.Rules.Add(rule24);
        avoidanceSystem.Rules.Add(rule25);
        avoidanceSystem.Rules.Add(rule27);
        avoidanceSystem.Rules.Add(rule28);
        avoidanceSystem.Rules.Add(rule29);
    }
}
