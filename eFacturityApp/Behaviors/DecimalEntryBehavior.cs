using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace eFacturityApp.Behaviors
{
    public class DecimalEntryBehavior : Behavior<Entry>
    {
        private static string regex = @"^([0-9]+)?([,|\.])?([0-9]+)?$";

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                var match = Regex.Match(args.NewTextValue, regex);
                if (!match.Success)
                {
                    ((Entry)sender).Text = args.NewTextValue.Remove(args.NewTextValue.Length - 1);
                }

                //Limitar 2 decimales.
                if (args.NewTextValue.Contains("."))
                {
                    if (args.NewTextValue.Length - 1 - args.NewTextValue.IndexOf(".") > 2)
                    {
                        var s = args.NewTextValue.Substring(0, args.NewTextValue.IndexOf(".") + 2 + 1);
                        ((Entry)sender).Text = s;
                    }
                }
            }

        }
    }
}
