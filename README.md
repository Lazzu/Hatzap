This repo is in bitbucket because it offers free private repos, but I'm planning to switch to github when I decide to go public with the project. I'm planning to go public "soonish" or "when the code is polished enough" or something like that, which will probably be in very near future if we get the editor rolling good.

I've been following few principles during the development of the engine and framework:

* Released game runtime must require as few external libraries as possible, and preferably no native binary libraries at all. This is the main reason why I rolled with my own file format for 3d assets and textures and basically everything. Currently only things required is the engine dll and OpenTK dll.
* The engine must be allowed to be used the way the programmer wants it to be used. This means the programmer may want to ditch the scene tree and just fill the rendering queue by hand. The programmer might not want to use ECS, or wants to use his own ECS. Maybe he implemented his own camera. The programmer might also just want to push some vertices to the GPU and render them instantly with some shader. Some people also want to load their own png files run-time. The opposite extreme is that the programmer does not even want to write his own asset loading and just want to give the engine a list of scenes to be loaded, like in Unity.
* Toolset should be as comprehensive as possible. For example the editor should handle sprite atlasing, megatexturing, material managing, light map baking, and so on.
* Lightweight

The current situation is that the editor is basically nonexistent, 3d rendering features are about 30% done, and 2d rendering features are, well, I'm not sure if they are 70% done or just a placeholder. Basic game engine features (scripting, entities, etc) are about 15 - 20% complete. If someone knows if some stuff can be done better, I will not shed a tear if any of the code changes. 

What currently is needed urgently:

* Proper project planning, feature list, roadmap, loose time plan. I'm going to go into this when I'm 100% sure I get help with the engine.
* Editor should be at least planned properly before publishing
* Helping hands. I've basically done everything alone and in the evening after first making some games at work, so at times the development speed has been very slow at best. I've had a small break from the project for the past 1.5 months to get some other stuff done, but I will get more active soon.
* Documentation. API docs are probably 20% there if they get generated from xml comments in the code, but any other documentation is nonexistent since I've been the only programmer so far.

So yeah. I'm being honest, it's nowhere perfect or ready to use. Some basic stuff can be done as shown in the example projects in the repo, but I'd be lying if I said that games can already be built with it.