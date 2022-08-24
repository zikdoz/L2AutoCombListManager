namespace L2AutoCombListManager
{
    public static class ListExtensions
    {
        public static List< string > ExtractLines( this List< string > source
                                                 , int offset
                                                 , int count )
        {
            return source
                  .Skip( offset )
                  .Take( count )
                  .ToList();
        }

        public static List< string > RemoveLines( this List< string > source
                                                , int offset
                                                , int count )
        {
            return source
                  .Where( ( _, i ) => ( ( i < offset ) || ( i >= ( offset + count ) ) ) )
                  .ToList();
        }
    }

    internal class Program
    {
        static List< string > extractItemData( string file_path )
        {
            return File.ReadAllLines( file_path )
                       .Skip( 1 )
                       .Select( line => line.Split( '=', 2 )
                                            .Skip( 1 )
                                            .First() )
                       .ToList();
        }

        static int findItemPos( List< string > source
                              , int item_id )
        {
            var str_item_id = $"One={item_id}";

            return source
                  .Select( ( line, idx ) => ( line, idx ) )
                  .First( tuple => tuple.line.Contains( str_item_id ) )
                  .idx;
        }

        enum EMoveItemPosMode
        {
            BEFORE_TARGET
          , AFTER_TARGET
        }

        static List< string > moveItems( List< string > source
                                       , int from_item_id
                                       , int to_item_id
                                       , EMoveItemPosMode item_pos_mode
                                       , int target_item_id )
        {
            var from_item_pos = findItemPos( source, from_item_id );
            var to_item_pos = findItemPos( source, to_item_id );

            var target_items_count = to_item_pos - from_item_pos + 1;

            var rest = source.RemoveLines( from_item_pos, target_items_count );
            var target = source.ExtractLines( from_item_pos, target_items_count );

            var target_item_pos = findItemPos( rest, target_item_id );

            if ( item_pos_mode == EMoveItemPosMode.AFTER_TARGET )
            {
                ++target_item_pos;
            }

            rest.InsertRange( target_item_pos, target );

            return rest;
        }

        static List< string > appendDyes( List< string > source
                                        , int start_item_id
                                        , int zero_item_id )
        {
            var dyes_list = new List< string >();
            const int DYES_COUNT = 20;

            for ( int i = -1; i < DYES_COUNT; ++i )
            {
                var powder_item = 96623;

                if ( i >= 10 )
                {
                    powder_item = 96630;
                }

                var target_item_id = start_item_id + i;
                var result_item_id = target_item_id + 1;

                if ( i == -1 )
                {
                    target_item_id = zero_item_id;
                    result_item_id = start_item_id;
                }

                dyes_list.Add( $"\"One={target_item_id}\tTwo={powder_item}\tResult={result_item_id}\tIgnore=0\tChance=2500\"" );
            }

            source.AddRange( dyes_list );

            return source;
        }

        static void dumpResult( string file_path
                              , IEnumerable< string > source )
        {
            File.WriteAllText( file_path, $"[list]{Environment.NewLine}" );

            File.AppendAllLines( file_path, source.Select( ( line, i ) => $"ID{i + 1}={line}" ) );
        }

        static void Main( string[] args )
        {
            var item_data = extractItemData( "backup_CombinationList.ini" );

            var move_talisman_braclets = moveItems( item_data, 9589, 92031, EMoveItemPosMode.AFTER_TARGET, 91304 );
            var move_novice_jewel = moveItems( move_talisman_braclets, 91936, 91936, EMoveItemPosMode.BEFORE_TARGET, 90311 );
            var move_agathion_bracelets = moveItems( move_novice_jewel, 90888, 91161, EMoveItemPosMode.AFTER_TARGET, 92031 );
            var move_einhasad_pendants = moveItems( move_agathion_bracelets, 93296, 93299, EMoveItemPosMode.AFTER_TARGET, 91161 );
            var move_hardin_crystalls = moveItems( move_einhasad_pendants, 95803, 95821, EMoveItemPosMode.AFTER_TARGET, 96919 );

            appendDyes( move_hardin_crystalls, 96355, 96617 ); // STR
            appendDyes( move_hardin_crystalls, 96375, 96618 ); // CON
            appendDyes( move_hardin_crystalls, 96395, 96619 ); // DEX
            appendDyes( move_hardin_crystalls, 96415, 96620 ); // INT
            appendDyes( move_hardin_crystalls, 96435, 96621 ); // MEN
            appendDyes( move_hardin_crystalls, 96455, 96622 ); // WIT

            var result_items = move_hardin_crystalls;
            dumpResult( "CombinationList.ini", result_items );
        }
    }
}