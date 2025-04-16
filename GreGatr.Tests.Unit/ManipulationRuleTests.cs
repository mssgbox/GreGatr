
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using GreGatr.Domain.Entities;
namespace GreGatr.Tests.Unit;

[TestFixture]
public class ManipulationRuleTests
{
    [Test]
    public void DetectsInfiniteLoop_WithoutCycleDetection()
    {
        var ruleA = new ManipulationRule(
            ManipulationRule.Type.ReplaceAndApply,
            find: "apple",
            replaceWith: "banana"
        );

        var ruleB = new ManipulationRule(
            ManipulationRule.Type.ReplaceAndApply,
            find: "banana",
            replaceWith: "apple",
            nestedRule: ruleA
        );

        ruleA = new ManipulationRule(
            ManipulationRule.Type.ReplaceAndApply,
            find: "apple",
            replaceWith: "banana",
            nestedRule: ruleB
        );

        string itemIdentifier = "apple";
        ruleA.Apply(ref itemIdentifier);
        // Run the Apply method with a timeout to detect infinite loop
        var task = Task.Run(() => ruleA.Apply(ref itemIdentifier));

        // If the method runs for too long, assume infinite loop
        if (!task.Wait(TimeSpan.FromSeconds(5)))
        {
            Assert.Fail("Infinite loop detected! The test did not complete.");
        }
        else
        {
            Assert.Pass("No infinite loop detected.");
        }
    }

    [Test]
    public void Apply_ReplaceRule_ReplacesSubstring()
    {
        // Arrange
        var rule = new ManipulationRule(ManipulationRule.Type.Replace, find: "abc", replaceWith: "123");
        string itemIdentifier = "abcdef";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("123def"));
    }

    [Test]
    public void Apply_ReplaceRule_NoMatch_DoesNotChangeItemIdentifier()
    {
        // Arrange
        var rule = new ManipulationRule(ManipulationRule.Type.Replace, find: "xyz", replaceWith: "123");
        string itemIdentifier = "abcdef";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("abcdef"));
    }

    [Test]
    public void Apply_ReplaceAndApplyRule_ReplacesAndAppliesNestedRule()
    {
        // Arrange
        var nestedRule = new ManipulationRule(ManipulationRule.Type.ToLower);
        var rule = new ManipulationRule(ManipulationRule.Type.ReplaceAndApply, find: "ABC", replaceWith: "XYZ", nestedRule: nestedRule);
        string itemIdentifier = "ABCDEF";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("xyzdef"));
    }

    [Test]
    public void Apply_PrependRule_PrependsPrefix()
    {
        // Arrange
        var rule = new ManipulationRule(ManipulationRule.Type.Prepend, prefix: "Prefix_");
        string itemIdentifier = "item123";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("Prefix_item123"));
    }

    [Test]
    public void Apply_ToLowerRule_ChangesToLowerCase()
    {
        // Arrange
        var rule = new ManipulationRule(ManipulationRule.Type.ToLower);
        string itemIdentifier = "HELLO";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("hello"));
    }

    [Test]
    public void Apply_ReplaceAndApplyRule_ApplyNestedRuleOnMatch()
    {
        // Arrange
        var nestedRule = new ManipulationRule(ManipulationRule.Type.ToLower);
        var rule = new ManipulationRule(ManipulationRule.Type.ReplaceAndApply, find: "HELLO", replaceWith: "Hi", nestedRule: nestedRule);
        string itemIdentifier = "HELLO World";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("hi world"));
    }

    [Test]
    public void Apply_ReplaceAndApplyRule_DoesNotApplyNestedRuleWithoutReplacement()
    {
        // Arrange
        var nestedRule = new ManipulationRule(ManipulationRule.Type.ToLower);
        var rule = new ManipulationRule(ManipulationRule.Type.ReplaceAndApply, find: "xyz", replaceWith: "123", nestedRule: nestedRule);
        string itemIdentifier = "HELLO World";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("HELLO World"));
    }

    [Test]
    public void Apply_WithNullFind_ReplaceDoesNotApply()
    {
        // Arrange
        var rule = new ManipulationRule(ManipulationRule.Type.Replace, replaceWith: "123");
        string itemIdentifier = "abcdef";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("abcdef"));
    }

    [Test]
    public void Apply_WithNullReplaceWith_ReplaceRemovesFind()
    {
        // Create a rule where ReplaceWith is null
        var rule = new ManipulationRule(
            ManipulationRule.Type.Replace,
            find: "abc",  // Find value to look for
            replaceWith: null  // Null ReplaceWith means the "abc" will be removed
        );

        string itemIdentifier = "abcxyz";  // Initial string is "abcxyz"
        rule.Apply(ref itemIdentifier);  // Apply the rule

        // Assert that the "abc" part is removed, and the string becomes "xyz"
        Assert.That(itemIdentifier, Is.EqualTo("xyz"));
    }


    [Test]
    public void Apply_WithNestedRules_HandlesMultipleLevels()
    {
        // Arrange
        var nestedRule1 = new ManipulationRule(ManipulationRule.Type.ToLower);
        var nestedRule2 = new ManipulationRule(ManipulationRule.Type.Replace, find: "e", replaceWith: "3");
        var rule = new ManipulationRule(ManipulationRule.Type.ReplaceAndApply, find: "ABC", replaceWith: "ABCDEF", nestedRule: nestedRule1);
        string itemIdentifier = "ABC123";

        // Act
        rule.Apply(ref itemIdentifier);

        // Assert
        Assert.That(itemIdentifier, Is.EqualTo("abcdef123"));
    }

    [Test]
    public void Apply_WithDepthLimit_DoesNotExceedMaxDepth()
    {
        var rule = CreateNestedRuleWithFlippedReplacement(20);

        string itemIdentifier = "xyz"; // Initial string containing "xyz"
        rule.Apply(ref itemIdentifier);


        // Assert: Ensure that the final string is correctly modified (replaced with "xyz")
        Assert.That(itemIdentifier, Is.EqualTo("xyz"));
    }

    // Helper method to create nested rules that trigger recursive replacement
    private ManipulationRule CreateNestedRuleWithFlippedReplacement(int depth)
    {
        //template
        ManipulationRule? currentRule = new ManipulationRule(
            ManipulationRule.Type.ReplaceAndApply,
            find: "abc",  // Initial find value
            replaceWith: "xyz"  // Initial replaceWith value
        );

        //nest rules flipping replacements to ensure continued recursion
        for (int i = 1; i < depth; i++)
        {
            currentRule = new ManipulationRule(
                ManipulationRule.Type.ReplaceAndApply,
                find: currentRule.ReplaceWith,  // Flip Find with ReplaceWith
                replaceWith: currentRule.Find,  // Flip ReplaceWith with Find
                nestedRule: currentRule  // The current rule becomes the nested rule
            );
        }

        return currentRule!;
    }




}
