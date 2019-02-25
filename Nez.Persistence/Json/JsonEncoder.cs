﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace Nez.Persistence
{
	/// <summary>
	/// responsible for serializing an object to JSON
	/// </summary>
	public sealed class JsonEncoder : IJsonEncoder
	{
		static readonly StringBuilder _builder = new StringBuilder( 2000 );
		readonly CacheResolver _cacheResolver = new CacheResolver();
		readonly JsonSettings _settings;
		readonly Dictionary<object, int> _referenceTracker = new Dictionary<object, int>();
		int _referenceCounter = 0;
		int indent;

		/// <summary>
		/// maintains state on whether the first item of an object/array was written. This lets use know if we
		/// need to stick a comma in between values
		/// </summary>
		Stack<bool> _isFirstItemWrittenStack = new Stack<bool>();


		public static string ToJson( object obj, JsonSettings settings )
		{
			var encoder = new JsonEncoder( settings );
			encoder.EncodeValue( obj, false );

			var json = _builder.ToString();
			_builder.Length = 0;
			return json;
		}

		JsonEncoder( JsonSettings settings )
		{
			_settings = settings;
			indent = 0;
		}

		public void EncodeValue( object value, bool forceTypeHint = false )
		{
			if( value == null )
			{
				_builder.Append( "null" );
			}
			else if( value is string )
			{
				WriteString( (string)value );
			}
			else if( value is ProxyString )
			{
				WriteString( ( (ProxyString)value ).ToString( CultureInfo.InvariantCulture ) );
			}
			else if( value is char )
			{
				WriteString( value.ToString() );
			}
			else if( value is bool )
			{
				_builder.Append( (bool)value ? "true" : "false" );
			}
			else if( value is Enum )
			{
				WriteString( value.ToString() );
			}
			else if( value is Array )
			{
				EncodeArray( (Array)value );
			}
			else if( value is IList )
			{
				EncodeList( (IList)value );
			}
			else if( value is IDictionary )
			{
				EncodeDictionary( (IDictionary)value );
			}
			else if( value is ProxyArray )
			{
				EncodeProxyArray( (ProxyArray)value );
			}
			else if( value is ProxyObject )
			{
				EncodeProxyObject( (ProxyObject)value );
			}
			else if( value is float ||
				value is double ||
				value is int ||
				value is uint ||
				value is long ||
				value is sbyte ||
				value is byte ||
				value is short ||
				value is ushort ||
				value is ulong ||
				value is decimal ||
				value is ProxyBoolean ||
				value is ProxyNumber )
			{
				_builder.Append( Convert.ToString( value, CultureInfo.InvariantCulture ) );
				return;
			}
			else
			{
				EncodeObject( value, forceTypeHint );
			}
		}

		public void EncodeObject( object value, bool forceTypeHint )
		{
			var type = value.GetType();

			WriteStartObject();

			if( WriteOptionalReferenceData( value ) )
			{
				WriteEndObject();
				return;
			}

			WriteOptionalTypeHint( type, forceTypeHint );

			// check for an override converter and use it if present
			var converter = _settings.GetTypeConverterForType( type );
			if( converter != null && converter.WantsWrite )
			{
				converter.WriteJson( this, value );
			}

			foreach( var field in _cacheResolver.GetEncodableFieldsForType( type ) )
			{
				WriteValueDelimiter();
				WriteString( field.Name );
				AppendColon();
				EncodeValue( field.GetValue( value ) );
			}

			foreach( var property in _cacheResolver.GetEncodablePropertiesForType( type ) )
			{
				if( property.CanRead )
				{
					WriteValueDelimiter();
					WriteString( property.Name );
					AppendColon();
					EncodeValue( property.GetValue( value, null ) );
				}
			}

			WriteEndObject();
		}

		void EncodeProxyArray( ProxyArray value )
		{
			WriteStartArray();

			foreach( var obj in value )
			{
				WriteValueDelimiter();
				EncodeValue( obj );
			}

			WriteEndArray();
		}

		void EncodeProxyObject( ProxyObject value )
		{
			WriteStartObject();

			foreach( var e in value.Keys )
			{
				WriteValueDelimiter();
				WriteString( e );
				AppendColon();
				EncodeValue( value[e] );
			}

			WriteEndObject();
		}

		public void EncodeDictionary( IDictionary value )
		{
			WriteStartObject();

			foreach( var e in value.Keys )
			{
				WriteValueDelimiter();
				WriteString( e.ToString() );
				AppendColon();
				EncodeValue( value[e] );
			}

			WriteEndObject();
		}

		public void EncodeList( IList value )
		{
			var forceTypeHint = _settings.TypeNameHandling == TypeNameHandling.All || _settings.TypeNameHandling == TypeNameHandling.Arrays;

			Type listItemType = null;
			// auto means we need to know the item type of the list. If our object is not the same as the list type
			// then we need to put the type hint in.
			if( !forceTypeHint && _settings.TypeNameHandling == TypeNameHandling.Auto )
			{
				var listType = value.GetType();
				foreach( Type interfaceType in listType.GetInterfaces() )
				{
					if( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof( IList<> ) )
					{
						listItemType = listType.GetGenericArguments()[0];
						break;
					}
				}
			}

			WriteStartArray();

			foreach( var obj in value )
			{
				WriteValueDelimiter();
				forceTypeHint = forceTypeHint || ( listItemType != null && listItemType != obj.GetType() );
				EncodeValue( obj, forceTypeHint );
			}

			WriteEndArray();
		}

		public void EncodeArray( Array value )
		{
			if( value.Rank == 1 )
			{
				EncodeList( value );
			}
			else
			{
				var indices = new int[value.Rank];
				EncodeArrayRank( value, 0, indices );
			}
		}

		public void EncodeArrayRank( Array value, int rank, int[] indices )
		{
			WriteStartArray();

			var min = value.GetLowerBound( rank );
			var max = value.GetUpperBound( rank );

			if( rank == value.Rank - 1 )
			{
				var forceTypeHint = _settings.TypeNameHandling == TypeNameHandling.All || _settings.TypeNameHandling == TypeNameHandling.Arrays;

				Type arrayItemType = null;
				if( _settings.TypeNameHandling == TypeNameHandling.Auto || _settings.TypeNameHandling == TypeNameHandling.Arrays )
				{
					arrayItemType = value.GetType().GetElementType();
				}

				for( var i = min; i <= max; i++ )
				{
					indices[rank] = i;
					WriteValueDelimiter();
					var val = value.GetValue( indices );
					forceTypeHint = forceTypeHint || ( arrayItemType != null && arrayItemType != val.GetType() );
					EncodeValue( val, forceTypeHint );
				}
			}
			else
			{
				for( var i = min; i <= max; i++ )
				{
					indices[rank] = i;
					WriteValueDelimiter();
					EncodeArrayRank( value, rank + 1, indices );
				}
			}

			WriteEndArray();
		}


		#region Writers

		public void AppendIndent()
		{
			for( var i = 0; i < indent; i++ )
			{
				_builder.Append( '\t' );
			}
		}

		public void AppendColon()
		{
			_builder.Append( ':' );

			if( _settings.PrettyPrint )
			{
				_builder.Append( ' ' );
			}
		}

		/// <summary>
		/// uses the JsonSettings and object details to deal with reference tracking. If a reference was found and written
		/// to the JSON stream it will return true and the rest of the object data should not be written.
		/// </summary>
		/// <param name="value">Value.</param>
		public bool WriteOptionalReferenceData( object value )
		{
			if( _settings.PreserveReferencesHandling )
			{
				if( !_referenceTracker.ContainsKey( value ) )
				{
					_referenceTracker[value] = ++_referenceCounter;

					WriteValueDelimiter();
					WriteString( Json.IdPropertyName );
					AppendColon();
					WriteString( _referenceCounter.ToString() );
				}
				else
				{
					var id = _referenceTracker[value];

					WriteValueDelimiter();
					WriteString( Json.RefPropertyName );
					AppendColon();
					WriteString( id.ToString() );

					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// optionally writes the type hint
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="forceTypeHint">If set to <c>true</c> force type hint.</param>
		public void WriteOptionalTypeHint( Type type, bool forceTypeHint )
		{
			forceTypeHint = forceTypeHint || _settings.TypeNameHandling == TypeNameHandling.All || _settings.TypeNameHandling == TypeNameHandling.Objects;
			if( forceTypeHint )
			{
				WriteValueDelimiter();
				WriteString( Json.TypeHintPropertyName );
				AppendColon();
				WriteString( type.FullName );
			}
		}

		/// <summary>
		/// writes a comma when needed. Call this before writing any object/array key-value pairs.
		/// </summary>
		public void WriteValueDelimiter()
		{
			if( _isFirstItemWrittenStack.Peek() )
			{
				_builder.Append( ',' );

				if( _settings.PrettyPrint )
				{
					_builder.Append( '\n' );
				}
			}
			else
			{
				_isFirstItemWrittenStack.Pop();
				_isFirstItemWrittenStack.Push( true );
			}

			if( _settings.PrettyPrint )
			{
				AppendIndent();
			}
		}

		public void WriteStartObject()
		{
			_isFirstItemWrittenStack.Push( false );
			_builder.Append( '{' );

			if( _settings.PrettyPrint )
			{
				_builder.Append( '\n' );
				indent++;
			}
		}

		public void WriteEndObject()
		{
			_isFirstItemWrittenStack.Pop();
			if( _settings.PrettyPrint )
			{
				_builder.Append( '\n' );
				indent--;
				AppendIndent();
			}

			_builder.Append( '}' );
		}

		public void WriteStartArray()
		{
			_isFirstItemWrittenStack.Push( false );
			_builder.Append( '[' );

			if( _settings.PrettyPrint )
			{
				_builder.Append( '\n' );
				indent++;
			}
		}

		public void WriteEndArray()
		{
			_isFirstItemWrittenStack.Pop();
			if( _settings.PrettyPrint )
			{
				_builder.Append( '\n' );
				indent--;
				AppendIndent();
			}

			_builder.Append( ']' );
		}

		public void WriteString( string value )
		{
			_builder.Append( '\"' );

			var charArray = value.ToCharArray();
			foreach( var c in charArray )
			{
				switch( c )
				{
					case '"':
						_builder.Append( "\\\"" );
						break;

					case '\\':
						_builder.Append( "\\\\" );
						break;

					case '\b':
						_builder.Append( "\\b" );
						break;

					case '\f':
						_builder.Append( "\\f" );
						break;

					case '\n':
						_builder.Append( "\\n" );
						break;

					case '\r':
						_builder.Append( "\\r" );
						break;

					case '\t':
						_builder.Append( "\\t" );
						break;

					default:
						var codepoint = Convert.ToInt32( c );
						if( ( codepoint >= 32 ) && ( codepoint <= 126 ) )
						{
							_builder.Append( c );
						}
						else
						{
							_builder.Append( "\\u" + Convert.ToString( codepoint, 16 ).PadLeft( 4, '0' ) );
						}

						break;
				}
			}

			_builder.Append( '\"' );
		}

		#endregion

	}
}
