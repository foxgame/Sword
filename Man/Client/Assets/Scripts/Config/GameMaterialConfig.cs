using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaterialConfig : Singleton< GameMaterialConfig >
{
    Material[] materials;

    public override void initSingleton()
    {
        materials = new Material[ 16 ];

        materials[ 0 ] = Resources.Load<Material>( "Material/Type0" );
        materials[ 1 ] = Resources.Load<Material>( "Material/Type1" );
        materials[ 2 ] = Resources.Load<Material>( "Material/Type2" );
        materials[ 3 ] = Resources.Load<Material>( "Material/Type3" );
        materials[ 4 ] = Resources.Load<Material>( "Material/Type4" );
        materials[ 5 ] = Resources.Load<Material>( "Material/Type5" );
        materials[ 6 ] = Resources.Load<Material>( "Material/Type6" );
        materials[ 7 ] = Resources.Load<Material>( "Material/Type7" );
    }

    public Material getMaterial( int i )
    {
        return materials[ i ];
    }

}

