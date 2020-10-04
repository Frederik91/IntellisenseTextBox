# Intellisense Text Box
![Nuget](https://img.shields.io/nuget/v/IntellisenseTextBox?style=for-the-badge)

A text box that supports objects.

![Screenshot](https://github.com/Frederik91/IntellisenseTextBox/blob/main/Screenshot_1.png "Screenshot")

## Getting started
1. Add the nuget package to your project from here: [https://www.nuget.org/packages/IntellisenseTextBox/](https://www.nuget.org/packages/IntellisenseTextBox/)
2. Add using statement to xaml file: 
 ```xml 
  xmlns:controls="clr-namespace:SmartTextBox.Controls;assembly=SmartTextBox" 
  ```
3. Add control to your view:
 ```xml 
<controls:IntellisenseTextBox/>
  ```
  
## Configuration

### Properties
* ItemsSource: Bind to items to show in intellisense list
 ```xml 
<controls:IntellisenseTextBox ItemsSource="{Binding IntellisenseItems}"/>
  ```
* EnableDetails: Allow user to double click on items in text box to show detail view (true/false)
 ```xml 
<controls:IntellisenseTextBox EnableDetails="true"/>
  ```
  * Segments: List of segments in the text box. A segment is either a TextSegment or a ObjectSegment. This represents the content of the text box.
 ```xml 
<controls:IntellisenseTextBox Segments="{Binding Segments}"/>
  ```
    
* IntellisenseTrigger: String to match before showing intellisense popup. Defaults to "@".
 ```xml 
<controls:IntellisenseTextBox IntellisenseTrigger="@"/>
  ```
  
* SearchText: The text entered after the IntellisenseTrigger, eg. if the user types "@asd" then the search text is "asd".
 ```xml 
<controls:IntellisenseTextBox SearchText="{Binding SearchText}"/>
  ```
  
* ListItemTemplate: Overrides the data template for list items in intellisense popup.
 ```xml 
<controls:IntellisenseTextBox>
  <controls:IntellisenseTextBox.ListItemTemplate>
      <DataTemplate>
          <Border CornerRadius="2"
                  Margin="0,1,0,-3"
                  Padding="2,0,2,0"
                  Background="Transparent"
                  BorderBrush="Orange"
                  VerticalAlignment="Center"
                  BorderThickness="1">
              <TextBlock Text="{Binding DisplayText}"></TextBlock>
          </Border>
      </DataTemplate>
  </controls:IntellisenseTextBox.ListItemTemplate>
</controls:IntellisenseTextBox>
  ```
  
* GroupStyle: Overrides the group styles for list in intellisense popup.
 ```xml 
<controls:IntellisenseTextBox>
  <controls:IntellisenseTextBox.GroupStyle>
      <GroupStyle>
          <GroupStyle.HeaderTemplate>
              <DataTemplate>
                  <TextBlock FontWeight="Bold" FontSize="14" Text="{Binding Name}"/>
              </DataTemplate>
          </GroupStyle.HeaderTemplate>
      </GroupStyle>
  </controls:IntellisenseTextBox.GroupStyle>
</controls:IntellisenseTextBox>
  ```
  
* ItemTemplate: Overrides the styles for each object in the text box.
 ```xml 
<controls:IntellisenseTextBox>
    <controls:IntellisenseTextBox.ItemTemplate>
        <DataTemplate>
            <Border CornerRadius="2"
                    Margin="0,1,0,-3"
                    Padding="2,0,2,0"
                    Background="Transparent"
                    BorderBrush="Orange"
                    VerticalAlignment="Center"
                    BorderThickness="1">
                <TextBlock Text="{Binding DisplayText}"></TextBlock>
            </Border>
        </DataTemplate>
    </controls:IntellisenseTextBox.ItemTemplate>
</controls:IntellisenseTextBox>
  ```
  
* ItemTemplate: Overrides the data template for the detail view
 ```xml 
<controls:IntellisenseTextBox>
  <controls:IntellisenseTextBox.DetailItemTemplate>
    <controls:IntellisenseTextBox.DetailItemTemplate>
        <DataTemplate>
            <Grid Background="White">
                <TextBlock Text="{Binding DisplayText}"></TextBlock>
            </Grid>
        </DataTemplate>
    </controls:IntellisenseTextBox.DetailItemTemplate>
</controls:IntellisenseTextBox>
  ```
