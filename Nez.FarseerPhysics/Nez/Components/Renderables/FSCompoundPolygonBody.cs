﻿using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common.Decomposition;
using Nez.Textures;
using FarseerPhysics.Common.PolygonManipulation;


namespace Nez.Farseer
{
	/// <summary>
	/// creates a compound polygon based on an image
	/// </summary>
	public class FSCompoundPolygonBody : FSRenderableBody
	{
		protected List<Vertices> _verts = new List<Vertices>();


		public FSCompoundPolygonBody( Subtexture subtexture ) : base( subtexture )
		{}


		public override void initialize()
		{
			base.initialize();

			var data = new uint[_subtexture.sourceRect.Width * _subtexture.sourceRect.Height];
			_subtexture.texture2D.GetData( 0, _subtexture.sourceRect, data, 0, data.Length );

			var verts = PolygonTools.CreatePolygon( data, _subtexture.sourceRect.Width );
			verts = SimplifyTools.DouglasPeuckerSimplify( verts, 2 );

			var decomposedVerts = Triangulate.ConvexPartition( verts, TriangulationAlgorithm.Bayazit );
			for( var i = 0; i < decomposedVerts.Count; i++ )
			{
				var polygon = decomposedVerts[i];
				polygon.Translate( -_subtexture.center );
			}

			// add the fixtures
			var fixtures = body.attachCompoundPolygon( decomposedVerts, 1 );

			// fetch all the Vertices and save a copy in case we need to scale them later
			foreach( var fixture in fixtures )
				_verts.Add( new Vertices( ( fixture.Shape as PolygonShape ).Vertices ) );
		}


		public override void onEntityTransformChanged( Transform.Component comp )
		{
			base.onEntityTransformChanged( comp );
			if( _ignoreTransformChanges )
				return;

			// we only care about scale. base handles pos/rot
			if( comp == Transform.Component.Scale )
			{
				// fetch the Vertices, clear them, add our originals and scale them
				for( var i = 0; i < body.FixtureList.Count; i++ )
				{
					var poly = body.FixtureList[i].Shape as PolygonShape;
					var verts = poly.Vertices;
					verts.Clear();
					verts.AddRange( _verts[i] );
					verts.Scale( transform.scale );
					poly.Vertices = verts;
				}
			}
		}

	}
}