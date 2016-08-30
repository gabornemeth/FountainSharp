//TODO: these aren't right.
#if INTERACTIVE
#r "../../bin/FountainSharp.Parse.dll"
#r "../../packages/NUnit/lib/nunit.framework.dll"
#else
module FountainSharp.Tests.Parsing
#endif

open FsUnit
open NUnit.Framework
open FountainSharp.Parse

let properNewLines (text: string) = text.Replace("\r\n", System.Environment.NewLine)

//===== Block Elements ==============================================================

//===== Scene Headings
[<Test>]
let ``Basic Scene Heading`` () =
   let doc = "EXT. BRICK'S PATIO - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("EXT. BRICK'S PATIO - DAY"); range = Range(24, 0)}])]

[<Test>]
let ``Forced (".") Scene Heading`` () =
   let doc = ".BINOCULARS A FORCED SCENE HEADING - LATER" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (true, [{value = Literal ("BINOCULARS A FORCED SCENE HEADING - LATER"); range = Range(41, 0)}])]

[<Test>]
let ``Lowercase known scene heading`` () =
   let doc = "ext. brick's pool - day" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("ext. brick's pool - day"); range = Range(23, 0)}])]

[<Test>]
let ``Known INT Scene Head`` () =
   let doc = "INT DOGHOUSE - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("INT DOGHOUSE - DAY"); range = Range(18, 0)}])]

[<Test>]
let ``Known EXT Scene Head`` () =
   let doc = "EXT DOGHOUSE - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("EXT DOGHOUSE - DAY"); range = Range(18, 0)}])]

[<Test>]
let ``Known EST Scene Head`` () =
   let doc = "EST DOGHOUSE - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("EST DOGHOUSE - DAY"); range = Range(18, 0)}])]

[<Test>]
let ``Known INT./EXT Scene Head`` () =
   let doc = "INT./EXT DOGHOUSE - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("INT./EXT DOGHOUSE - DAY"); range = Range(23, 0)}])]

[<Test>]
let ``Known INT/EXT Scene Head`` () =
   let doc = "INT/EXT DOGHOUSE - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("INT/EXT DOGHOUSE - DAY"); range = Range(22, 0)}])]

[<Test>]
let ``Known I/E Scene Head`` () =
   let doc = "I/E DOGHOUSE - DAY" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("I/E DOGHOUSE - DAY"); range = Range(18, 0)}])]

[<Test>]
let ``Scene Heading with line breaks and action`` () =
   let doc = "EXT. BRICK'S PATIO - DAY\r\n\r\nSome Action" |> Fountain.Parse
   doc.Blocks
   |> should equal  [SceneHeading (false, [{value = Literal ("EXT. BRICK'S PATIO - DAY"); range = Range(24, 0)}]) 
                     Action (false, [{value = HardLineBreak; range = Range(1, 24)}; {value = Literal ("Some Action"); range = Range(11, 25)}])]


[<Test>]
let ``Scene Heading with more line breaks and action`` () =
   let doc = "EXT. BRICK'S PATIO - DAY\r\n\r\n\r\nSome Action" |> Fountain.Parse
   doc.Blocks
   |> should equal  [SceneHeading (false, [{value = Literal ("EXT. BRICK'S PATIO - DAY"); range = Range(24, 0)}])
                     Action(false, [{value = HardLineBreak; range = Range(1, 24)}; {value = HardLineBreak; range = Range(1, 25)}
                                    {value = Literal("Some Action"); range = Range(11, 26)}])]

//===== Action
[<Test>]
let ``Action with line breaks`` () =
   let doc = "EXT. BRICK'S PATIO - DAY\r\n\r\nSome Action\r\n\r\nSome More Action" |> Fountain.Parse
   doc.Blocks
   |> should equal [SceneHeading (false, [{value = Literal ("EXT. BRICK'S PATIO - DAY"); range = Range(0, 24)}])
                    Action (false, [{value = HardLineBreak; range = Range(1, 24)}
                                    {value = Literal ("Some Action"); range = Range(11, 25)}
                                    {value = HardLineBreak; range = Range(1, 36)}
                                    {value = HardLineBreak; range = Range(1, 37)}
                                    {value = Literal ("Some More Action"); range = Range(16, 38)}])]

[<Test>]
let ``Action with line breaks and no heading`` () =
   let doc = "Natalie looks around at the group, TIM, ROGER, NATE, and VEEK.\n
TIM, is smiling broadly." |> Fountain.Parse
   doc.Blocks
   |> should equal  [Action (false, [{value = Literal "Natalie looks around at the group, TIM, ROGER, NATE, and VEEK."; range = Range(0, 62)}
                                     {value = HardLineBreak; range = Range(1, 62)}; {value = HardLineBreak; range = Range(1, 63)} 
                                     {value = Literal "TIM, is smiling broadly."; range = Range(24, 64)}])]




//===== Synopses

[<Test>]
let ``Basic Synopses`` () =
   let doc = "= Here is a synopses of this fascinating scene." |> Fountain.Parse
   doc.Blocks
   |> should equal [Synopses ([{value = Literal " Here is a synopses of this fascinating scene."; range = Range(46, 0)}])]


//===== Character

[<Test>]
let ``Character - Normal`` () =
   let doc = "LINDSEY" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal "LINDSEY"; range = Range(7, 0)}])]

[<Test>]
let ``Character - With parenthetical extension`` () =
   let doc = "LINDSEY (on the radio)" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal ("LINDSEY (on the radio)"); range = Range(22, 0)}])]

[<Test>]
let ``Character - With whitespace`` () =
   let doc = "THIS IS ALL UPPERCASE BUT HAS WHITESPACE" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal ("THIS IS ALL UPPERCASE BUT HAS WHITESPACE"); range = Range(40, 0)}])]
    
[<Test>]
let ``Character - With Numbers`` () =
   let doc = "R2D2" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal ("R2D2"); range = Range(4, 0)}])]

[<Test>]
let ``Character - Number first`` () =
   let doc = "25D2" |> Fountain.Parse
   doc.Blocks
   |> should equal [Action (false, [{value = Literal ("25D2"); range = Range(4, 0)}])]

[<Test>]
let ``Character - Forced with at sign`` () =
   let doc = "@McAvoy" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (true, [{value = Literal ("McAvoy"); range = Range(6, 0)}])]

[<Test>]
let ``Character - with forced at and parenthetical extension`` () =
   let doc = "@McAvoy (OS)" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (true, [{value = Literal ("McAvoy (OS)"); range = Range(11, 0)}])]


//===== Parenthetical

[<Test>]
let ``Parenthetical `` () =
   let doc = "LINDSEY\r\n(quietly)" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal "LINDSEY"; range = Range(7, 0)}]); Parenthetical ([{value = Literal "quietly"; range = Range(7, 7)}])];

[<Test>]
let ``Parenthetical - After Dialogue`` () =
   let doc = "LINDSEY\r\n(quietly)\r\nHello, friend.\r\n(loudly)\r\nFriendo!" |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal "LINDSEY"; range = Range(7, 0)}]); Parenthetical ([{value = Literal "quietly"; range = Range(7, 7)}]) 
                    Dialogue ([{value = Literal "Hello, friend."; range = Range(14, 14)}]); Parenthetical ([{value = Literal ("loudly"); range = Range(6, 28)}])
                    Dialogue ([{value = Literal ("Friendo!"); range = Range(34, 8)}])];


//===== Dialogue

[<Test>]
let ``Dialogue - Normal`` () =
   let doc = "LINDSEY\r\nHello, friend." |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal ("LINDSEY"); range = Range(7, 0)}]); Dialogue ([{value = Literal ("Hello, friend."); range = Range(14, 7)}])]

[<Test>]
let ``Dialogue - After Parenthetical`` () =
   let doc = "LINDSEY\r\n(quietly)\r\nHello, friend." |> Fountain.Parse
   doc.Blocks
   |> should equal [Character (false, [{value = Literal ("LINDSEY"); range = Range(7, 0)}]); Parenthetical ([{value = Literal ("quietly"); range = Range(7, 7)}]) 
                    Dialogue ([{value = Literal ("Hello, friend."); range = Range(14, 14)}])]


//===== Page Break

[<Test>]
let ``PageBreak - ===`` () =
   let doc = "===" |> Fountain.Parse
   doc.Blocks
   |> should equal [PageBreak]

[<Test>]
// TODO: should this be a synopses? probably, yeah? need clarification from the spec
let ``PageBreak - == (not enough =)`` () =
   let doc = "==" |> Fountain.Parse
   doc.Blocks
   |> should equal [Synopses ([{value = Literal ("="); range = Range(1, 0)}])]

[<Test>]
let ``PageBreak - ==========`` () =
   let doc = "==========" |> Fountain.Parse
   doc.Blocks
   |> should equal [PageBreak]

[<Test>]
let ``PageBreak - ======= (with space at end)`` () =
   let doc = "======= " |> Fountain.Parse
   doc.Blocks
   |> should equal [PageBreak]


[<Test>]
let ``PageBreak - ======= blah (fail with other chars)`` () =
   let doc = "======= blah" |> Fountain.Parse
   doc.Blocks
   |> should equal [Synopses ([{value = Literal ("====== blah"); range = Range(11, 0)}])]


//===== Lyrics

[<Test>]
let ``Lyric - normal`` () =
   let doc = "~Birdy hop, he do. He hop a long." |> Fountain.Parse
   doc.Blocks
   |> should equal [Lyric ([{value = Literal ("Birdy hop, he do. He hop a long."); range = Range(32, 0)}])]


//===== Transition

[<Test>]
let ``Transition - normal`` () =
   let doc = "CUT TO:" |> Fountain.Parse
   doc.Blocks
   |> should equal [Transition (false, [{value = Literal ("CUT TO:"); range = Range(7, 0)}])]

[<Test>]
let ``Transition - forced`` () =
   let doc = "> Burn to White." |> Fountain.Parse
   doc.Blocks
   |> should equal [Transition (true, [{value = Literal ("Burn to White."); range = Range(14, 0)}])]

//===== Centered

[<Test>]
let ``Centered `` () =
   let doc = ">The End<" |> Fountain.Parse
   doc.Blocks
   |> should equal [Centered ([{value = Literal ("The End"); range = Range(7, 0)}])]

// TODO: wtf doesn't this compile either?
[<Test>]
let ``Centered - with spaces`` () =
   let doc = "> The End <" |> Fountain.Parse
   doc.Blocks
   |> should equal [Centered ([{value = Literal ("The End"); range = Range(7, 0)}])]

//===== Line Breaks

[<Test>]
let ``Line Breaks`` () =
   let doc = "Murtaugh, springing...\n\nAn explosion of sound...\nAs it rises like an avenging angel ...\nHovers, shattering the air \n\nScreaming, chaos, frenzy.\nThree words that apply to this scene." |> Fountain.Parse
   doc.Blocks
   |> should equal "" (*[Action (false, [Literal "Murtaugh, springing..."; HardLineBreak; HardLineBreak; Literal "An explosion of sound..."; HardLineBreak; 
      Literal "As it rises like an avenging angel ..."; HardLineBreak;Literal "Hovers, shattering the air"; HardLineBreak; HardLineBreak;
      Literal "Screaming, chaos, frenzy."; HardLineBreak; Literal "Three words that apply to this scene."])]*)
  


//===== Notes
[<Test>]
let ``Notes - Inline`` () =
   let doc = "Some text and then a [[bit a of a note]]. And some more text." |> Fountain.Parse
   doc.Blocks
   // TODO: figure out the right output here. 
   |> should equal [Action (false, [{value = Literal "fails anyway, it drops the note right now."; range = Range(41, 0)}])]
   //|> should equal [Action [Literal "Some text and then a "];[Note [Literal"bit of a note"]]; Literal ". And some more text."]

[<Test>]
let ``Notes - Block`` () =
   let doc = "[[It was supposed to be Vietnamese, right?]]" |> Fountain.Parse
   doc.Blocks
   |> should equal [Note [{value = Literal "It was supposed to be Vietnamese, right?"; range = Range(40, 0)}]]

//===== Boneyard (Comments)
//TODO: not implemented yet.

//===== Span Elements ==============================================================

//===== Emphasis

[<Test>]
let ``Emphasis - Bold`` () =
   let doc = "**This is bold Text**" |> Fountain.Parse
   doc.Blocks
   |> should equal [Action (false, [{value = Strong [{value = Literal ("This is bold Text"); range = Range(17, 0)}]; range = Range(17, 0)}])]

[<Test>]
let ``Emphasis - Italics`` () =
   let doc = "*This is italic Text*" |> Fountain.Parse
   doc.Blocks
   |> should equal [Action (false, [{value = Italic [{value = Literal "This is italic Text"; range = Range(19, 0)}]; range = Range(19, 0)}])]

[<Test>]
let ``Emphasis - Bold Italic`` () =
   let doc = "***This is bold Text***" |> Fountain.Parse
   doc.Blocks
   |> should equal [Action (false, [{value = Strong [{value = Italic [{value = Literal "This is bold Text"; range = Range(17, 0)}]; range = Range(17, 0)}]
                                     range = Range(17, 0)}])]

//TODO: i don't even know how to write this test. it fails anyway. (look at next one, for clues)
//[<Test>]
//let ``Emphasis - Nested Bold Italic`` () =
// let doc = "**This is bold *and Italic Text***" |> Fountain.Parse
// doc.Blocks
//   |> should equal [Action (false, [Literal ("need to figure out how to write the value here.")])]
// |> should equal [Action [Strong [Literal "This is bold "] [Italic [Literal "and Italic Text"]]]]

//[<Test>]
//let ``Emphasis - Nested Italic Bold`` () =
//   let doc = "From what seems like only INCHES AWAY.  _Steel's face FILLS the *Leupold Mark 4* scope_." |> Fountain.Parse
//   doc.Blocks
//   |> should equal ""//[Action (false, [Literal ("From what seems like only INCHES AWAY.  ")]; Underline ([Literal ("Steel's face FILLS the "); Italic [Literal ("Leupold Mark 4")]; Literal (" scope")]; Literal ("."))]

[<Test>]
let ``Emphasis - with escapes`` () =
   let doc = "Steel enters the code on the keypad: **\*9765\***" |> Fountain.Parse
   doc.Blocks
   |> should equal ""//[Action (false, [Literal ("Steel enters the code on the keypad: ")]

[<Test>]
let ``Emphasis - italics with spaces to left`` () =
   let doc = "He dialed *69 and then *23, and then hung up." |> Fountain.Parse
   doc.Blocks
   |> should equal ""//[Action (false, [Literal "He dialed "; Italic [Literal "69 and then "]; Literal "23, and then hung up."])]

[<Test>]
let ``Emphasis - italics with spaces to left but escaped`` () =
   let doc = "He dialed *69 and then 23\*, and then hung up.." |> Fountain.Parse
   doc.Blocks
   |> should equal ""// [Action (false, [Literal "He dialed *69 and then 23*, and then hung up.."])]

// TODO: are line breaks being recognized properly here? i think not. i think i need more line break cases
[<Test>]
let ``Emphasis - between line breaks`` () =
   let doc = "As he rattles off the long list, Brick and Steel *share a look.\r\n\
             \r\n\
             This is going to be BAD.*" |> Fountain.Parse
   doc.Blocks
   |> should equal ""//[Action (false, [Literal @"As he rattles off the long list, Brick and Steel *share a look."; HardLineBreak; HardLineBreak; Literal "This is going to be BAD.*"])]

//===== Indenting
// TODO

