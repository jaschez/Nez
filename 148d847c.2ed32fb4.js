(window.webpackJsonp=window.webpackJsonp||[]).push([[5],{108:function(e,t,n){"use strict";n.d(t,"a",(function(){return c})),n.d(t,"b",(function(){return f}));var r=n(0),a=n.n(r);function i(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function l(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){i(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,r,a=function(e,t){if(null==e)return{};var n,r,a={},i=Object.keys(e);for(r=0;r<i.length;r++)n=i[r],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(r=0;r<i.length;r++)n=i[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var u=a.a.createContext({}),d=function(e){var t=a.a.useContext(u),n=t;return e&&(n="function"==typeof e?e(t):l(l({},t),e)),n},c=function(e){var t=d(e.components);return a.a.createElement(u.Provider,{value:t},e.children)},p={inlineCode:"code",wrapper:function(e){var t=e.children;return a.a.createElement(a.a.Fragment,{},t)}},h=a.a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,i=e.originalType,o=e.parentName,u=s(e,["components","mdxType","originalType","parentName"]),c=d(n),h=r,f=c["".concat(o,".").concat(h)]||c[h]||p[h]||i;return n?a.a.createElement(f,l(l({ref:t},u),{},{components:n})):a.a.createElement(f,l({ref:t},u))}));function f(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var i=n.length,o=new Array(i);o[0]=h;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l.mdxType="string"==typeof e?e:r,o[1]=l;for(var u=2;u<i;u++)o[u]=n[u];return a.a.createElement.apply(null,o)}return a.a.createElement.apply(null,n)}h.displayName="MDXCreateElement"},70:function(e,t,n){"use strict";n.r(t),n.d(t,"frontMatter",(function(){return i})),n.d(t,"metadata",(function(){return o})),n.d(t,"toc",(function(){return l})),n.d(t,"default",(function(){return u}));var r=n(3),a=(n(0),n(108));const i={id:"DeferredLighting",title:"Deferred Lighting"},o={unversionedId:"features/Graphics/DeferredLighting",id:"features/Graphics/DeferredLighting",isDocsHomePage:!1,title:"Deferred Lighting",description:"The Deferred Lighting system lets you use real lights in your scene. Included light types are point, directional, spot and area lights.",source:"@site/docs\\features\\Graphics\\DeferredLighting.md",slug:"/features/Graphics/DeferredLighting",permalink:"/Nez/docs/features/Graphics/DeferredLighting",editUrl:"https://github.com/prime31/Nez/edit/master/Nez.github.io/docs/features/Graphics/DeferredLighting.md",version:"current",sidebar:"someSidebar",previous:{title:"Rendering",permalink:"/Nez/docs/features/Graphics/Rendering"},next:{title:"Nez SVG Support",permalink:"/Nez/docs/features/Graphics/SVG"}},l=[{value:"Material Setup",id:"material-setup",children:[]},{value:"Scene Setup",id:"scene-setup",children:[]},{value:"Entity Setup",id:"entity-setup",children:[]}],s={toc:l};function u({components:e,...t}){return Object(a.b)("wrapper",Object(r.a)({},s,t,{components:e,mdxType:"MDXLayout"}),Object(a.b)("p",null,"The Deferred Lighting system lets you use real lights in your scene. Included light types are point, directional, spot and area lights."),Object(a.b)("p",null,"Deferred lighting is a method of rendering that defers dealing with lights until after all objects are rendered. This makes having lots of lights more efficient. The general gist of how it works is like this: first, we render all objects to a diffuse render texture and render their normal maps to a normal render texture simultaneously (via multiple render targets). All of the lights are then rendered using the normal data into a 3rd render texture. Finally, the light render texture and the diffuse render texture are combined for the final output. You can see the output of all of the render textures by toggling the ",Object(a.b)("inlineCode",{parentName:"p"},"DeferredRenderer.EnableDebugBufferRender")," property at runtime."),Object(a.b)("h2",{id:"material-setup"},"Material Setup"),Object(a.b)("p",null,"This is where using the deferred lighting system differs from your normal workflow when making a 2D game. In most games without realtime lighting you wont ever need to touch the Material system at all. Deferred lighting needs additional information (mainly normal maps) in order to calculate lighting so we have to dive into Nez's Material system. We won't get into actual asset creation process here so it is assumed that you know how to create (or auto-generate) your normal maps."),Object(a.b)("p",null,"One neat trick the Nez deferred lighting system has baked in is self illumination. Self illuminated portions of your texture don't need to have a light hit them for them to be visible. They are always lit. You can specify which portions of your texture should be self illuminated via the alpha channel of your normal map. A 0 value indicates no self illumination and a value of 1 indicates fully self illuminated. Don't forget that premultiplied alpha must not be used when using self illumination! Nez needs that alpha channel intact to get the self illumination data. When using self illumination you have to let Nez know by setting ",Object(a.b)("inlineCode",{parentName:"p"},"DeferredSpriteMaterial.SetUseNormalAlphaChannelForSelfIllumination"),". Additional control of self illumination at runtime is available by setting ",Object(a.b)("inlineCode",{parentName:"p"},"DeferredSpriteMaterial.SetSelfIlluminationPower"),". Modulating the self illumination power lets you add some great atmosphere to a scene."),Object(a.b)("p",null,'There are going to be times that you don\'t want your objects normal mapped or maybe you havent created your normal maps yet. The deferred lighting system can accommodate this as well. There is a built-in "null normal map texture" that you can configure in your Material that will make any object using it only take part in diffuse lighting. By default, ',Object(a.b)("inlineCode",{parentName:"p"},"DeferredLightingRenderer.material")," will be a Material containing the null normal map. Whenever a Renderer encounters a RenderableComponent with a null Material it will use it's own Material. What that all means is that if you add a RenderableComponent with a null Material it will be rendered with just diffuse lighting."),Object(a.b)("p",null,"Below are the three most common Material setups: normal mapped lit, normal mapped lit self illuminated and only diffuse (no normal map)."),Object(a.b)("pre",null,Object(a.b)("code",{parentName:"pre",className:"language-csharp"},"// lit, normal mapped Material. normalMapTexture is a reference to the Texture2D that contains your normal map.\nvar standardMaterial = new DeferredSpriteMaterial( normalMapTexture );\n\n\n// diffuse lit Material. The NullNormalMapTexture is used\nvar diffuseOnlylMaterial = new DeferredSpriteMaterial( deferredRenderer.NullNormalMapTexture );\n\n\n// lit, normal mapped and self illuminated Material.\n// first we create the Material with our normal map. Note that your normal map should have an alpha channel for the self illumination and it\n// needs to have premultiplied alpha disabled\nvar selfLitMaterial = new DeferredSpriteMaterial( selfLitNormalMapTexture );\n\n// we can access the Effect on a Material<T> via the `TypedEffect` property. We need to tell the Effect that we want self illumination and\n// optionally set the self illumination power.\nselfLitMaterial.effect.SetUseNormalAlphaChannelForSelfIllumination( true )\n    .SetSelfIlluminationPower( 0.5f );\n")),Object(a.b)("h2",{id:"scene-setup"},"Scene Setup"),Object(a.b)("p",null,"There isn't much that needs to be done for your Scene setup. All you have to do is add a ",Object(a.b)("inlineCode",{parentName:"p"},"DeferredLightingRenderer"),". The values you pass to the constructor of this Renderer are very important though! You have to specify which RenderLayer it should use for lights and which RenderLayers contain your normal sprites."),Object(a.b)("pre",null,Object(a.b)("code",{parentName:"pre",className:"language-csharp"},"// define your renderLayers somewhere easy to access\nconst int LIGHT_LAYER = 1;\nconst int OBJECT_LAYER1 = 10;\nconst int OBJECT_LAYER2 = 20\n\n// add the DeferredLightingRenderer to your Scene specifying which renderLayer contains your lights and an arbitrary number of renderLayers for it to render\nvar deferredRenderer = scene.AddRenderer( new DeferredLightingRenderer( 0, LIGHT_LAYER, OBJECT_LAYER1, OBJECT_LAYER2 ) );\n\n// optionally set ambient lighting\ndeferredRenderer.SetAmbientColor( Color.Black );\n")),Object(a.b)("h2",{id:"entity-setup"},"Entity Setup"),Object(a.b)("p",null,"Now we just have to make sure that we use the proper RenderLayers (easy to do since we were smart and made them ",Object(a.b)("inlineCode",{parentName:"p"},"const int"),") and Materials when creating our Renderables:"),Object(a.b)("pre",null,Object(a.b)("code",{parentName:"pre",className:"language-csharp"},'// create an Entity to house our sprite\nvar entity = Scene.CreateEntity( "sprite" );\n\n// add a Sprite and here is the important part: be sure to set the renderLayer and material\nentity.AddComponent( new Sprite( spriteTexture ) )\n    .SetRenderLayer( OBJECT_LAYER1 )\n    .SetMaterial( standardMaterial );\n\n\n// create an entity with no Material set. It will use the DeferredLightingRenderer.Material which is diffuse only by default\nscene.CreateEntity( "diffuse-only" )\n    .AddComponent( new Sprite( spriteTexture ) )\n    .SetRenderLayer( OBJECT_LAYER1 );\n\n\n// create an Entity to house our PointLight\nvar lightEntity = scene.CreateEntity( "point-light" );\n\n// add a PointLight Component and be sure to set the renderLayer to the lights layer!\nlightEntity.AddComponent( new PointLight( Color.Yellow ) )\n    .SetRenderLayer( LIGHT_LAYER );\n')))}u.isMDXComponent=!0}}]);