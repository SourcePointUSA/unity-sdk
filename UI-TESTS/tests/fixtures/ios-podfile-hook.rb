# Execute the generated Ruby against the CocoaPods target/configuration boundary.
def platform(*)
end
def target(*)
  yield
end
def pod(*)
end
def post_install(&block)
  raise 'Duplicate post_install hook' if @hook
  @hook = block
end
load ARGV[0]
Config = Struct.new(:build_settings)
Target = Struct.new(:name, :build_configurations)
Project = Struct.new(:targets)
Installer = Struct.new(:pods_project)
names = %w[ConsentViewController SPMobileCore ConsentViewController-ConsentViewController UnrelatedPod ConsentViewControllerOther]
targets = names.map { |name| Target.new(name, [Config.new({'IPHONEOS_DEPLOYMENT_TARGET' => '10.0', 'OTHER' => 'retained'}), Config.new({'IPHONEOS_DEPLOYMENT_TARGET' => '18.0'})]) }
@hook.call(Installer.new(Project.new(targets)))
targets.each_with_index do |target, index|
  expected = index < 3 ? ARGV[1] : '10.0'
  raise "Wrong floor for #{target.name}" unless target.build_configurations[0].build_settings['IPHONEOS_DEPLOYMENT_TARGET'] == expected
  raise "Lowered higher floor for #{target.name}" unless target.build_configurations[1].build_settings['IPHONEOS_DEPLOYMENT_TARGET'] == '18.0'
  raise 'Changed unrelated build setting' unless target.build_configurations[0].build_settings['OTHER'] == 'retained'
end
puts 'PASS: generated Ruby raises only CMP/core/resource targets and preserves higher/unrelated settings'
