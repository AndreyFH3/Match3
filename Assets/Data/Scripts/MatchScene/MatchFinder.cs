using System.Collections.Generic;
using System.Linq;

public static class MatchFinder
{
    public static List<GemField> FindAllMatches(MatchField field)
    {
        HashSet<GemField> allMatches = new HashSet<GemField>();
        
        for (int y = 0; y < GetMaxY(field) + 1; y++)
        {
            List<GemField> horizontalMatch = FindHorizontalMatch(field, y);
            foreach (var matchField in horizontalMatch)
            {
                allMatches.Add(matchField);
            }
        }
        
        for (int x = 0; x < GetMaxX(field) + 1; x++)
        {
            List<GemField> verticalMatch = FindVerticalMatch(field, x);
            foreach (var matchField in verticalMatch)
            {
                allMatches.Add(matchField);
            }
        }
        
        List<GemField> squareMatches = FindSquareMatches(field);
        foreach (var matchField in squareMatches)
        {
            allMatches.Add(matchField);
        }
        
        List<GemField> cornerMatches = FindCornerMatches(field);
        foreach (var matchField in cornerMatches)
        {
            allMatches.Add(matchField);
        }
        
        return allMatches.ToList();
    }

    public static List<GemField> FindHorizontalMatch(MatchField field, int y)
    {
        List<GemField> matches = new List<GemField>();
        List<GemField> currentSequence = new List<GemField>();
        GemType? currentType = null;
        
        for (int x = 0; x <= GetMaxX(field); x++)
        {
            GemField gemField = field.GetField(x, y);
            if (gemField == null || gemField.CurrentGem == null)
            {
                if (currentSequence.Count >= 3)
                {
                    matches.AddRange(currentSequence);
                }
                currentSequence.Clear();
                currentType = null;
                continue;
            }
            
            GemType type = gemField.CurrentGem.Type;
            
            if (currentType == null || currentType == type)
            {
                currentSequence.Add(gemField);
                currentType = type;
            }
            else
            {
                if (currentSequence.Count >= 3)
                {
                    matches.AddRange(currentSequence);
                }
                currentSequence = new List<GemField> { gemField };
                currentType = type;
            }
        }
        
        if (currentSequence.Count >= 3)
        {
            matches.AddRange(currentSequence);
        }
        
        return matches;
    }

    public static List<GemField> FindVerticalMatch(MatchField field, int x)
    {
        List<GemField> matches = new List<GemField>();
        List<GemField> currentSequence = new List<GemField>();
        GemType? currentType = null;
        
        for (int y = 0; y <= GetMaxY(field); y++)
        {
            GemField gemField = field.GetField(x, y);
            if (gemField == null || gemField.CurrentGem == null)
            {
                if (currentSequence.Count >= 3)
                {
                    matches.AddRange(currentSequence);
                }
                currentSequence.Clear();
                currentType = null;
                continue;
            }
            
            GemType type = gemField.CurrentGem.Type;
            
            if (currentType == null || currentType == type)
            {
                currentSequence.Add(gemField);
                currentType = type;
            }
            else
            {
                if (currentSequence.Count >= 3)
                {
                    matches.AddRange(currentSequence);
                }
                currentSequence = new List<GemField> { gemField };
                currentType = type;
            }
        }
        
        if (currentSequence.Count >= 3)
        {
            matches.AddRange(currentSequence);
        }
        
        return matches;
    }

    public static List<GemField> FindSquareMatches(MatchField field)
    {
        List<GemField> matches = new List<GemField>();
        
        for (int x = 0; x < GetMaxX(field); x++)
        {
            for (int y = 0; y < GetMaxY(field); y++)
            {
                GemField topLeft = field.GetField(x, y);
                GemField topRight = field.GetField(x + 1, y);
                GemField bottomLeft = field.GetField(x, y + 1);
                GemField bottomRight = field.GetField(x + 1, y + 1);
                
                if (topLeft?.CurrentGem != null && topRight?.CurrentGem != null &&
                    bottomLeft?.CurrentGem != null && bottomRight?.CurrentGem != null)
                {
                    GemType type = topLeft.CurrentGem.Type;
                    
                    if (topRight.CurrentGem.Type == type &&
                        bottomLeft.CurrentGem.Type == type &&
                        bottomRight.CurrentGem.Type == type)
                    {
                        matches.Add(topLeft);
                        matches.Add(topRight);
                        matches.Add(bottomLeft);
                        matches.Add(bottomRight);
                    }
                }
            }
        }
        
        return matches;
    }

    public static List<GemField> FindCornerMatches(MatchField field)
    {
        List<GemField> matches = new List<GemField>();
        
        for (int x = 0; x <= GetMaxX(field); x++)
        {
            for (int y = 0; y <= GetMaxY(field); y++)
            {
                GemField corner = field.GetField(x, y);
                if (corner?.CurrentGem == null) continue;
                
                GemType cornerType = corner.CurrentGem.Type;
                List<GemField> lShape = CheckLShape(field, x, y, cornerType);
                
                if (lShape.Count == 5)
                {
                    matches.AddRange(lShape);
                }
            }
        }
        
        return matches.Distinct().ToList();
    }

    private static List<GemField> CheckLShape(MatchField field, int cornerX, int cornerY, GemType type)
    {
        List<GemField> lShape = new List<GemField>();
        
        
        if (CheckLShapeUpLeft(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeUpRight(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeDownLeft(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeDownRight(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeLeftUp(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeLeftDown(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeRightUp(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();
        
        if (CheckLShapeRightDown(field, cornerX, cornerY, type, ref lShape)) return lShape;
        lShape.Clear();

        return lShape;
    }

    private static bool CheckLShapeUpLeft(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {        
        GemField up1 = field.GetField(cornerX, cornerY - 1);
        GemField up2 = field.GetField(cornerX, cornerY - 2);
        GemField left1 = field.GetField(cornerX - 1, cornerY);
        
        if (up1?.CurrentGem != null && up1.CurrentGem.Type == type &&
            up2?.CurrentGem != null && up2.CurrentGem.Type == type &&
            left1?.CurrentGem != null && left1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(up1);
            lShape.Add(up2);
            lShape.Add(left1);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeUpRight(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField up1 = field.GetField(cornerX, cornerY - 1);
        GemField up2 = field.GetField(cornerX, cornerY - 2);
        GemField right1 = field.GetField(cornerX + 1, cornerY);
        
        if (up1?.CurrentGem != null && up1.CurrentGem.Type == type &&
            up2?.CurrentGem != null && up2.CurrentGem.Type == type &&
            right1?.CurrentGem != null && right1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(up1);
            lShape.Add(up2);
            lShape.Add(right1);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeDownLeft(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField down1 = field.GetField(cornerX, cornerY + 1);
        GemField down2 = field.GetField(cornerX, cornerY + 2);
        GemField left1 = field.GetField(cornerX - 1, cornerY);
        
        if (down1?.CurrentGem != null && down1.CurrentGem.Type == type &&
            down2?.CurrentGem != null && down2.CurrentGem.Type == type &&
            left1?.CurrentGem != null && left1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(down1);
            lShape.Add(down2);
            lShape.Add(left1);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeDownRight(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField down1 = field.GetField(cornerX, cornerY + 1);
        GemField down2 = field.GetField(cornerX, cornerY + 2);
        GemField right1 = field.GetField(cornerX + 1, cornerY);
        GemField right2 = field.GetField(cornerX + 2, cornerY);
        
        if (down1?.CurrentGem != null && down1.CurrentGem.Type == type &&
            down2?.CurrentGem != null && down2.CurrentGem.Type == type &&
            right1?.CurrentGem != null && right1.CurrentGem.Type == type &&
            right2?.CurrentGem != null && right2.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(down1);
            lShape.Add(down2);
            lShape.Add(right1);
            lShape.Add(right2);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeLeftUp(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField left1 = field.GetField(cornerX - 1, cornerY);
        GemField left2 = field.GetField(cornerX - 2, cornerY);
        GemField up1 = field.GetField(cornerX, cornerY - 1);
        
        if (left1?.CurrentGem != null && left1.CurrentGem.Type == type &&
            left2?.CurrentGem != null && left2.CurrentGem.Type == type &&
            up1?.CurrentGem != null && up1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(left1);
            lShape.Add(left2);
            lShape.Add(up1);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeLeftDown(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField left1 = field.GetField(cornerX - 1, cornerY);
        GemField left2 = field.GetField(cornerX - 2, cornerY);
        GemField down1 = field.GetField(cornerX, cornerY + 1);
        
        if (left1?.CurrentGem != null && left1.CurrentGem.Type == type &&
            left2?.CurrentGem != null && left2.CurrentGem.Type == type &&
            down1?.CurrentGem != null && down1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(left1);
            lShape.Add(left2);
            lShape.Add(down1);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeRightUp(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField right1 = field.GetField(cornerX + 1, cornerY);
        GemField right2 = field.GetField(cornerX + 2, cornerY);
        GemField up1 = field.GetField(cornerX, cornerY - 1);
        
        if (right1?.CurrentGem != null && right1.CurrentGem.Type == type &&
            right2?.CurrentGem != null && right2.CurrentGem.Type == type &&
            up1?.CurrentGem != null && up1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(right1);
            lShape.Add(right2);
            lShape.Add(up1);
            return true;
        }
        return false;
    }

    private static bool CheckLShapeRightDown(MatchField field, int cornerX, int cornerY, GemType type, ref List<GemField> lShape)
    {
        GemField right1 = field.GetField(cornerX + 1, cornerY);
        GemField right2 = field.GetField(cornerX + 2, cornerY);
        GemField down1 = field.GetField(cornerX, cornerY + 1);
        
        if (right1?.CurrentGem != null && right1.CurrentGem.Type == type &&
            right2?.CurrentGem != null && right2.CurrentGem.Type == type &&
            down1?.CurrentGem != null && down1.CurrentGem.Type == type)
        {
            lShape.Add(field.GetField(cornerX, cornerY));
            lShape.Add(right1);
            lShape.Add(right2);
            lShape.Add(down1);
            return true;
        }
        return false;
    }

    public static int GetMaxX(MatchField field)
    {
        return field.Fields.Max(f => f.X);
    }

    public static int GetMaxY(MatchField field)
    {
        return field.Fields.Max(f => f.Y);
    }
}